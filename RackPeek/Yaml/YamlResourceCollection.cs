using System.Collections.Concurrent;
using System.Collections.Specialized;
using RackPeek.Domain.Resources;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Services;
using RackPeek.Domain.Resources.SystemResources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RackPeek.Yaml;

public sealed class YamlResourceCollection(bool watch) : IDisposable
{
    private static readonly TimeSpan ReloadDebounce = TimeSpan.FromMilliseconds(300);

    private readonly object _sync = new();

    private readonly List<ResourceEntry> _entries = [];
    private readonly List<string> _knownFiles = [];
    private readonly ConcurrentDictionary<string, DateTime> _reloadQueue = [];
    private readonly bool _watch = watch;
    private readonly Dictionary<string, FileSystemWatcher> _watchers = [];

    public IReadOnlyList<string> SourceFiles
    {
        get
        {
            lock (_sync)
                return _knownFiles.ToList();
        }
    }

    public IReadOnlyList<Hardware> HardwareResources
    {
        get
        {
            lock (_sync)
            {
                return _entries
                    .Select(e => e.Resource)
                    .OfType<Hardware>()
                    .ToList();
            }
        }
    }

    public IReadOnlyList<SystemResource> SystemResources
    {
        get
        {
            lock (_sync)
            {
                return _entries
                    .Select(e => e.Resource)
                    .OfType<SystemResource>()
                    .ToList();
            }
        }
    }

    public IReadOnlyList<Service> ServiceResources
    {
        get
        {
            lock (_sync)
            {
                return _entries
                    .Select(e => e.Resource)
                    .OfType<Service>()
                    .ToList();
            }
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            foreach (var watcher in _watchers.Values)
                watcher.Dispose();

            _watchers.Clear();
        }
    }

    // ----------------------------
    // Loading
    // ----------------------------

    public void LoadFiles(IEnumerable<string> filePaths)
    {
        foreach (var file in filePaths)
        {
            TrackFile(file);
            LoadFile(file);
        }
    }

    public void Load(string yaml, string file)
    {
        TrackFile(file);

        var newEntries = Deserialize(yaml)
            .Where(r => r != null)
            .Select(r => new ResourceEntry(r!, file))
            .ToList();

        lock (_sync)
        {
            RemoveEntriesFromFile(file);
            _entries.AddRange(newEntries);
        }
    }

    private void LoadFile(string file)
    {
        var yaml = File.Exists(file)
            ? SafeReadAllText(file)
            : string.Empty;

        var newEntries = Deserialize(yaml)
            .Where(r => r != null)
            .Select(r => new ResourceEntry(r!, file))
            .ToList();

        lock (_sync)
        {
            RemoveEntriesFromFile(file);
            _entries.AddRange(newEntries);
        }
    }

    // ----------------------------
    // Watching
    // ----------------------------

    private void TrackFile(string file)
    {
        lock (_sync)
        {
            if (!_knownFiles.Contains(file))
                _knownFiles.Add(file);

            var directory = Path.GetDirectoryName(file);
            if (directory == null || !_watch || _watchers.ContainsKey(directory))
                return;

            var watcher = new FileSystemWatcher(directory)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.Size
            };

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileRenamed;

            _watchers[directory] = watcher;
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_sync)
        {
            if (!_knownFiles.Contains(e.FullPath))
                return;
        }

        QueueReload(e.FullPath);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        lock (_sync)
        {
            if (_knownFiles.Contains(e.OldFullPath))
            {
                RemoveEntriesFromFile(e.OldFullPath);
                _knownFiles.Remove(e.OldFullPath);
            }
        }

        lock (_sync)
        {
            if (_knownFiles.Contains(e.FullPath))
                QueueReload(e.FullPath);
        }
    }

    private void QueueReload(string file)
    {
        _reloadQueue[file] = DateTime.UtcNow;

        Task.Delay(ReloadDebounce).ContinueWith(_ =>
        {
            if (_reloadQueue.TryGetValue(file, out var timestamp) &&
                DateTime.UtcNow - timestamp >= ReloadDebounce)
            {
                _reloadQueue.TryRemove(file, out var _);
                LoadFile(file);
            }
        });
    }

    // ----------------------------
    // CRUD
    // ----------------------------

    public void Add(Resource resource, string sourceFile)
    {
        TrackFile(sourceFile);

        lock (_sync)
        {
            _entries.Add(new ResourceEntry(resource, sourceFile));
        }
    }

    public void Update(Resource resource)
    {
        lock (_sync)
        {
            var existing = _entries.FirstOrDefault(e =>
                e.Resource.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
                throw new InvalidOperationException($"Resource '{resource.Name}' not found.");

            _entries.Remove(existing);
            _entries.Add(new ResourceEntry(resource, existing.SourceFile));
        }
    }

    public void Delete(string name)
    {
        lock (_sync)
        {
            var existing = _entries.FirstOrDefault(e =>
                e.Resource.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
                throw new InvalidOperationException($"Resource '{name}' not found.");

            _entries.Remove(existing);
        }
    }

    public Resource? GetByName(string name)
    {
        lock (_sync)
        {
            return _entries
                .Select(e => e.Resource)
                .FirstOrDefault(r =>
                    r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void RemoveEntriesFromFile(string file)
    {
        _entries.RemoveAll(e => e.SourceFile == file);
    }

    // ----------------------------
    // Serialization
    // ----------------------------

    public void SaveAll()
    {
        List<string> files;
        List<ResourceEntry> snapshot;

        lock (_sync)
        {
            files = _knownFiles.ToList();
            snapshot = _entries.ToList();
        }

        foreach (var file in files)
        {
            var resources = snapshot
                .Where(e => e.SourceFile == file)
                .Select(e => e.Resource);

            SaveToFile(file, resources);
        }
    }

    private static void SaveToFile(string filePath, IEnumerable<Resource> resources)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var payload = new OrderedDictionary
        {
            ["resources"] = resources.Select(SerializeResource).ToList()
        };

        File.WriteAllText(filePath, serializer.Serialize(payload));
    }

    private static OrderedDictionary SerializeResource(Resource resource)
    {
        var map = new OrderedDictionary
        {
            ["kind"] = resource switch
            {
                Server => "Server",
                Switch => "Switch",
                Firewall => "Firewall",
                Router => "Router",
                Desktop => "Desktop",
                Laptop => "Laptop",
                AccessPoint => "AccessPoint",
                Ups => "Ups",
                SystemResource => "System",
                Service => "Service",
                _ => throw new InvalidOperationException($"Unknown resource type: {resource.GetType().Name}")
            }
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(resource);

        var props = new DeserializerBuilder()
            .Build()
            .Deserialize<Dictionary<string, object?>>(yaml) ?? new();

        foreach (var (key, value) in props)
            if (key != "kind")
                map[key] = value;

        return map;
    }

    private static List<Resource> Deserialize(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml))
            return [];

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithCaseInsensitivePropertyMatching()
            .WithTypeConverter(new StorageSizeYamlConverter())
            .Build();

        var raw = deserializer.Deserialize<
            Dictionary<string, List<Dictionary<string, object>>>>(yaml);

        if (raw == null || !raw.TryGetValue("resources", out var items))
            return [];

        var resources = new List<Resource>();

        foreach (var item in items)
        {
            if (!item.TryGetValue("kind", out var kindObj) || kindObj == null)
                continue;

            var kind = kindObj.ToString();
            var typedYaml = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Serialize(item);

            Resource resource = kind switch
            {
                "Server" => deserializer.Deserialize<Server>(typedYaml),
                "Switch" => deserializer.Deserialize<Switch>(typedYaml),
                "Firewall" => deserializer.Deserialize<Firewall>(typedYaml),
                "Router" => deserializer.Deserialize<Router>(typedYaml),
                "Desktop" => deserializer.Deserialize<Desktop>(typedYaml),
                "Laptop" => deserializer.Deserialize<Laptop>(typedYaml),
                "AccessPoint" => deserializer.Deserialize<AccessPoint>(typedYaml),
                "Ups" => deserializer.Deserialize<Ups>(typedYaml),
                "System" => deserializer.Deserialize<SystemResource>(typedYaml),
                "Service" => deserializer.Deserialize<Service>(typedYaml),
                _ => null
            };

            if (resource != null)
                resources.Add(resource);
        }

        return resources;
    }

    private static string SafeReadAllText(string file)
    {
        for (var i = 0; i < 5; i++)
        {
            try
            {
                return File.ReadAllText(file);
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        return string.Empty;
    }

    private sealed record ResourceEntry(Resource Resource, string SourceFile);
}
