using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.Event
{
    public interface IUsbEventRepository
    {
        Task SaveEventAsync(UsbDeviceEventEntity evt);

        Task <UsbDeviceEventEntity?> GetLastEventAsync(string deviceId);

        Task <UsbDeviceEventEntity?> GetLastEventBySessionIdAsync(Guid sessionId); 
    }
}
