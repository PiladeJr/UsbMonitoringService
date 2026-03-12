using UsbMonitoringService.Models;

namespace UsbMonitoringService.Infrastructure.Storage
{
    public interface IDeviceRegistry
    {
        Task RegisterDeviceAsync(UsbDevice device);

        Task RemoveDeviceAsync(string deviceId);

        IReadOnlyCollection<UsbDeviceState> GetAllDevices();

        UsbDeviceState? GetDevice(string deviceId);

        Task RestoreDeviceAsync(UsbDevice device,Guid sessionId,long lastObservedUsedSpace);
    }
}
