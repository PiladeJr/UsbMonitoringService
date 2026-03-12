using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceInfo
{
    public class UsbDeviceRepository(IDbContextFactory<UsbMonitoringDbContext> contextFactory) : IUsbDeviceRepository
    {
        private readonly IDbContextFactory<UsbMonitoringDbContext> _contextFactory = contextFactory;

        public Task<bool> ExistsAsync(string deviceId)
        {
            return QueryByDeviceId(deviceId).AnyAsync();
        }

        public Task<UsbDeviceEntity?> GetByDeviceIdAsync(string deviceId)
        {
            return QueryByDeviceId(deviceId).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(UsbDeviceEntity device)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Devices.Add(device);
            await context.SaveChangesAsync();
        }

        private IQueryable<UsbDeviceEntity> QueryByDeviceId(string deviceId)
        {
            using var context = _contextFactory.CreateDbContextAsync();
            return context.Result.Devices.Where(d => d.Id == deviceId);
        }
    }
}
