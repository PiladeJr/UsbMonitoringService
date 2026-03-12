using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceInfo
{
    public interface IUsbDeviceRepository
    {
        Task<bool> ExistsAsync(string deviceId);
        Task<UsbDeviceEntity?> GetByDeviceIdAsync(string deviceId);
        Task InsertAsync(UsbDeviceEntity device);
    }
}
