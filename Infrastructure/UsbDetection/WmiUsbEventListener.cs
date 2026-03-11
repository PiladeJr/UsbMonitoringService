using System.Management;
using System.Runtime.Versioning;

namespace UsbMonitoringService.Infrastructure.UsbDetection
{
    [SupportedOSPlatform("windows")]
    public class WmiUsbEventListener : IUsbEventListener
    {
        private ManagementEventWatcher? _insertWatcher;
        private ManagementEventWatcher? _removeWatcher;

        public event Action<string>? UsbInserted;
        public event Action<string>? UsbRemoved;

        public void Start()
        {
            var insertQuery = new WqlEventQuery(
                "SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");

            _insertWatcher = new ManagementEventWatcher(insertQuery);
            _insertWatcher.EventArrived += OnUsbInserted;
            _insertWatcher.Start();

            var removeQuery = new WqlEventQuery(
                "SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");

            _removeWatcher = new ManagementEventWatcher(removeQuery);
            _removeWatcher.EventArrived += OnUsbRemoved;
            _removeWatcher.Start();
        }

        public void Stop()
        {
            _insertWatcher?.Stop();
            _removeWatcher?.Stop();
        }

        private void OnUsbInserted(object sender, EventArrivedEventArgs e)
        {
            var drive = e.NewEvent["DriveName"]?.ToString();

            if (!string.IsNullOrWhiteSpace(drive))
            {
                UsbInserted?.Invoke(drive);
            }
        }

        private void OnUsbRemoved(object sender, EventArrivedEventArgs e)
        {
            var drive = e.NewEvent["DriveName"]?.ToString();

            if (!string.IsNullOrWhiteSpace(drive))
            {
                UsbRemoved?.Invoke(drive);
            }
        }
    }
}
