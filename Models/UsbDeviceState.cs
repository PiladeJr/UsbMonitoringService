using UsbMonitoringService.Model;

namespace UsbMonitoringService.Models
{
    public class UsbDeviceState
    {
        public UsbDevice Device { get; set; }

        public long BaselineUsedSpace { get; set; }

        public long LastObservedUsedSpace { get; set; }
    }
}
