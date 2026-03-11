namespace UsbMonitoringService.Models
{
    public class SecurityEvent
    {
        public string EventType { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public long DataDelta { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
