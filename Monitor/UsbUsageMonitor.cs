using System.Runtime.Serialization;
using UsbMonitoringService.Infrastructure.Storage;

namespace UsbMonitoringService.Monitor
{
    public class UsbUsageMonitor(
        IDeviceRegistry deviceRegistry,
        ILogger<UsbUsageMonitor> logger)
    {
        private readonly IDeviceRegistry _deviceRegistry = deviceRegistry;
        private readonly ILogger<UsbUsageMonitor> _logger = logger;

        public void MonitorDevices()
        {
            var devices = _deviceRegistry.GetAllDevices();

            foreach (var state in devices)
            {
                try
                {
                    var mountPoint = state.Device.MountPoint;

                    long currentUsage = DriveUsageProvider.GetUsedSpace(mountPoint);

                    if (currentUsage < 0)
                        continue;

                    long delta = currentUsage - state.LastObservedUsedSpace;

                    if (delta > 0)
                    {
                        _logger.LogWarning(
                            "USB WRITE detected on {Device} | +{Bytes} bytes",
                            state.Device.Name,
                            delta);
                    }


                    if (delta < 0)
                    {
                        _logger.LogInformation(
                            "USB DELETE detected on {Device} | {Bytes} bytes",
                            state.Device.Name,
                            delta);
                    }

                    state.LastObservedUsedSpace = currentUsage;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error monitoring device");
                }
            }
        }
    }
}
