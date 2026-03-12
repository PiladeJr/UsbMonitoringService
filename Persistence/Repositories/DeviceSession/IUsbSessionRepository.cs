using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceSession
{
    public  interface IUsbSessionRepository
    {
        Task<Guid> CreateSessionAsync(string deviceId, long startUsedSpace);

        Task CloseSessionAsync(Guid sessionId, long endUsedSpace);

        Task<List<UsbDeviceSessionEntity>> GetOpenSessionsAsync();
    }
}
