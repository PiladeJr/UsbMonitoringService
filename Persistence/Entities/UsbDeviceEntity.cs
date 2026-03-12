namespace UsbMonitoringService.Persistence.Entities
{
    public class UsbDeviceEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FabricName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string MountPort { get; set; } = string.Empty;
    }
}
