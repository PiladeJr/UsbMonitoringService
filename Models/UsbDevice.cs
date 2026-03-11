namespace UsbMonitoringService.Models
{
    public class UsbDevice
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FabricName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string MountPoint { get; set; } = string.Empty;

        public long TotalSpace { get; set; }
        public long UsedSpace { get; set; }

        public DateTime InsertTimestamp { get; set; }
    }
}
