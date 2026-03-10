namespace UsbMonitoringService.Models
{
    public class SecurityEvent
    {
        public string EventType { get; set; }

        public string DeviceId { get; set; }

        public long DataDelta { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
