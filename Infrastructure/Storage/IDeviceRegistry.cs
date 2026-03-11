using UsbMonitoringService.Models;

namespace UsbMonitoringService.Infrastructure.Storage
{
    public interface IDeviceRegistry
    {
        void RegisterDevice(UsbDevice device);

        void RemoveDevice(string deviceId);

        IReadOnlyCollection<UsbDeviceState> GetAllDevices();

        UsbDeviceState? GetDevice(string deviceId);
    }
}
