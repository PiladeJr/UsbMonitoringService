using Microsoft.EntityFrameworkCore;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceInfo
{
    public class UsbDeviceRepository(UsbMonitoringDbContext context) : IUsbDeviceRepository
    {
        private readonly UsbMonitoringDbContext _context = context;

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
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
        }

        private IQueryable<UsbDeviceEntity> QueryByDeviceId(string deviceId)
        {
            return _context.Devices.Where(d => d.Id == deviceId);
        }
    }
}
