namespace UsbMonitoringService.Persistence.Entities
{
    public class UsbDeviceEventEntity
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; } 

        public string EventType { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public long Dimension { get; set; }

        public long UsedSpace { get; set; }
    }
}
