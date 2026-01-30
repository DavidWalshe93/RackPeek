using System.Collections.Specialized;
using System.Collections.Concurrent;
using RackPeek.Domain.Resources;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Services;
using RackPeek.Domain.Resources.SystemResources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RackPeek.Yaml;

public sealed class YamlResourceCollection(bool watch) : IDisposable
{
    private readonly bool _watch = watch;
    
    private readonly List<ResourceEntry> _entries = [];
    private readonly List<string> _knownFiles = [];
    private readonly Dictionary<string, FileSystemWatcher> _watchers = [];
    private readonly ConcurrentDictionary<string, DateTime> _reloadQueue = [];

    private static readonly TimeSpan ReloadDebounce = TimeSpan.FromMilliseconds(300);

    public IReadOnlyList<string> SourceFiles => _knownFiles.ToList();

    public IReadOnlyList<Hardware> HardwareResources =>
        _entries.Select(e => e.Resource).OfType<Hardware>().ToList();

    public IReadOnlyList<SystemResource> SystemResources =>
        _entries.Select(e => e.Resource).OfType<SystemResource>().ToList();

    public IReadOnlyList<Service> ServiceResources =>
        _entries.Select(e => e.Resource).OfType<Service>().ToList();

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
        foreach (var resource in Deserialize(yaml))
            _entries.Add(new ResourceEntry(resource, file));
    }

    private void LoadFile(string file)
    {
        RemoveEntriesFromFile(file);

        var yaml = File.Exists(file)
            ? SafeReadAllText(file)
            : string.Empty;

        foreach (var resource in Deserialize(yaml))
            _entries.Add(new ResourceEntry(resource, file));
    }

    // ----------------------------
    // Watching
    // ----------------------------

    private void TrackFile(string file)
    {
        if (!_knownFiles.Contains(file))
            _knownFiles.Add(file);

        var directory = Path.GetDirectoryName(file)!;

        if (_watchers.ContainsKey(directory) || !_watch)
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

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (!_knownFiles.Contains(e.FullPath))
            return;

        QueueReload(e.FullPath);
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (_knownFiles.Contains(e.OldFullPath))
        {
            RemoveEntriesFromFile(e.OldFullPath);
            _knownFiles.Remove(e.OldFullPath);
        }

        if (_knownFiles.Contains(e.FullPath))
            QueueReload(e.FullPath);
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
        _entries.Add(new ResourceEntry(resource, sourceFile));
    }

    public void Update(Resource resource)
    {
        var existing = _entries.FirstOrDefault(e =>
            e.Resource.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase));

        if (existing == null)
            throw new InvalidOperationException($"Resource '{resource.Name}' not found.");

        _entries.Remove(existing);
        _entries.Add(new ResourceEntry(resource, existing.SourceFile));
    }

    public void Delete(string name)
    {
        var existing = _entries.FirstOrDefault(e =>
            e.Resource.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existing == null)
            throw new InvalidOperationException($"Resource '{name}' not found.");

        _entries.Remove(existing);
    }

    public Resource? GetByName(string name)
    {
        return _entries
            .Select(e => e.Resource)
            .FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
        foreach (var file in _knownFiles)
        {
            var resources = _entries
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
            .Deserialize<Dictionary<string, object?>>(yaml);

        foreach (var (key, value) in props)
        {
            if (key != "kind")
                map[key] = value;
        }

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
            var kind = item["kind"].ToString();
            var typedYaml = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Serialize(item);

            resources.Add(kind switch
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
                _ => throw new InvalidOperationException($"Unknown kind: {kind}")
            });
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

    public void Dispose()
    {
        foreach (var watcher in _watchers.Values)
            watcher.Dispose();
    }

    private sealed record ResourceEntry(Resource Resource, string SourceFile);
}
