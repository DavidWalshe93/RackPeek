namespace RackPeek.Domain.Resources.Hardware.Models;

public class Drive
{
    public string? Type { get; set; }
    public int? Size { get; set; }
    
    public static readonly string[] ValidDriveTypes =
    {
        // Flash storage
        "nvme", "ssd",
        // Traditional spinning disks
        "hdd",
        // Enterprise interfaces
        "sas", "sata",
        // External / removable
        "usb", "sdcard", "micro-sd"
    };
    
}