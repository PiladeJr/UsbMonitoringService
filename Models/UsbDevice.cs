namespace UsbMonitoringService.Model
{
    public class UsbDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string MountPoint { get; set; }

        public long TotalSpace { get; set; }
        public long UsedSpace { get; set; }

        public DateTime InsertTimestamp { get; set; }
    }
}
