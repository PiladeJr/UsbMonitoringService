namespace UsbMonitoringService.Persistence.Entities
{
    public class UsbDeviceSessionEntity
    {
        public Guid Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public DateTime StartTimestamp { get; set; }

        public DateTime? EndTimestamp { get; set; }

        public long StartUsedSpace { get; set; }

        public long? EndUsedSpace { get; set; }
    }
}