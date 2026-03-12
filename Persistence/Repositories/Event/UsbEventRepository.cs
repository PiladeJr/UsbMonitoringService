using Microsoft.EntityFrameworkCore;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.Event
{
    public class UsbEventRepository(UsbMonitoringDbContext context) : IUsbEventRepository
    {
        private readonly UsbMonitoringDbContext _context = context;

        public async Task SaveEventAsync(UsbDeviceEventEntity evt)
        {
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task<UsbDeviceEventEntity?> GetLastEventAsync(string deviceId)
        {
            return await (from e in _context.Events
                          join s in _context.Session on e.SessionId equals s.Id
                          where s.DeviceId == deviceId
                          orderby e.Timestamp descending
                          select e)
                .FirstOrDefaultAsync();
        }

        public async Task<UsbDeviceEventEntity?> GetLastEventBySessionIdAsync(Guid sessionId)
        {
            return await _context.Events
                .Where(e => e.SessionId == sessionId)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
