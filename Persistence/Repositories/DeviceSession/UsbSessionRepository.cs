using Microsoft.EntityFrameworkCore;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceSession
{
    public class UsbSessionRepository(UsbMonitoringDbContext context) : IUsbSessionRepository
    {
        private readonly UsbMonitoringDbContext _context = context;

        public async Task<Guid> CreateSessionAsync(string deviceId, long startUsedSpace)
        {
            var session = new UsbDeviceSessionEntity
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                StartTimestamp = DateTime.UtcNow,
                StartUsedSpace = startUsedSpace
            };

            _context.Session.Add(session);
            await _context.SaveChangesAsync();

            return session.Id;
        }

        public async Task CloseSessionAsync(Guid sessionId, long endUsedSpace)
        {
            var session = await _context.Session.FindAsync(sessionId);

            if (session == null)
                return;

            session.EndTimestamp = DateTime.UtcNow;
            session.EndUsedSpace = endUsedSpace;

            await _context.SaveChangesAsync();
        }

        public async Task<List<UsbDeviceSessionEntity>> GetOpenSessionsAsync()
        {
            return await _context.Session
                .Where(s => s.EndTimestamp == null)
                .ToListAsync();
        }
    }
}
