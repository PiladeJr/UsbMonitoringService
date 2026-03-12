using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Models;
using UsbMonitoringService.Persistence.Entities;
using UsbMonitoringService.Persistence.Repositories.Event;

namespace UsbMonitoringService.Monitor
{
    public class UsbUsageMonitor(
        IDeviceRegistry deviceRegistry,
        ILogger<UsbUsageMonitor> logger,
        IUsbEventRepository eventRepo)
    {
        private readonly IDeviceRegistry _deviceRegistry = deviceRegistry;
        private readonly ILogger<UsbUsageMonitor> _logger = logger;
        private readonly IUsbEventRepository _eventRepo = eventRepo;

        public async Task MonitorDevices()
        {
            var devices = _deviceRegistry.GetAllDevices();

            foreach (var state in devices)
            {
                try
                {
                    if (state.Device == null)
                        continue;

                    if (_deviceRegistry.GetDevice(state.Device.Id) == null)
                        continue;

                    var mountPoint = state.Device.MountPoint;

                    long currentUsage;
                    try
                    {
                        currentUsage = DriveUsageProvider.GetUsedSpace(mountPoint);
                    }
                    catch
                    {
                        continue;
                    }

                    if (currentUsage < 0)
                        continue;

                    long delta = currentUsage - state.LastObservedUsedSpace;
                    var now = DateTime.UtcNow;

                    switch (delta)
                        {
                        case > 0:
                            await OnPositiveDelta(state, delta);
                            break;
                        case < 0:
                            await OnNegativeDelta(state, delta);
                            break;
                    }

                    var idleTime = now - state.LastWriteTimestamp;

                    if (state.PendingWriteBytes > 0 && idleTime > TimeSpan.FromSeconds(3))
                    {
                        await SaveWriteEvent(state);
                    }

                    state.LastObservedUsedSpace = currentUsage;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error monitoring device");
                }
            }
        }

        private async Task SaveWriteEvent(UsbDeviceState state) {
            
            if (state.Device == null)
                return;

            await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
            {
                SessionId = state.SessionId,
                EventType = "Write",
                Timestamp = DateTime.UtcNow,
                Dimension = state.PendingWriteBytes
            });

            // Reset pending bytes dopo il salvataggio
            state.PendingWriteBytes = 0;
        }

        private async Task OnPositiveDelta(UsbDeviceState state, long delta)
        {
            //Per ora lascio il warning per evidenziare le scritture, da valutare se spostarlo nel
            //salvataggio dell'evento per avere un evento unico con i byte totali scritti
#pragma warning disable CS8602 // Dereferenziamento di un possibile riferimento Null. 
            _logger.LogWarning(
                "USB WRITE detected on {Device} | +{Bytes} bytes",
                state.Device.Name,
                delta);
#pragma warning restore CS8602 

            state.PendingWriteBytes += delta;
            state.LastWriteTimestamp = DateTime.UtcNow;
        }

        private async Task OnNegativeDelta(UsbDeviceState state, long delta)
        {
            _logger.LogInformation(
                "USB DELETE detected on {Device} | {Bytes} bytes",
                state.Device?.Name,
                delta);
            await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
            {
                SessionId = state.SessionId,
                EventType = "Delete",
                Timestamp = DateTime.UtcNow,
                Dimension = delta
            });
        }
    }
}
