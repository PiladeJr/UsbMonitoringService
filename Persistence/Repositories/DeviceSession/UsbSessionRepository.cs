using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.DeviceSession
{
    public class UsbSessionRepository(IDbContextFactory<UsbMonitoringDbContext> contextFactory) : IUsbSessionRepository
    {
        private readonly IDbContextFactory<UsbMonitoringDbContext> _contextFactory = contextFactory;

        public async Task<Guid> CreateSessionAsync(string deviceId, long startUsedSpace)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var session = new UsbDeviceSessionEntity
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                StartTimestamp = DateTime.UtcNow,
                StartUsedSpace = startUsedSpace
            };

            context.Session.Add(session);
            await context.SaveChangesAsync();

            return session.Id;
        }

        public async Task CloseSessionAsync(Guid sessionId, long endUsedSpace)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var session = await context.Session.FindAsync(sessionId);

            if (session == null)
                return;

            session.EndTimestamp = DateTime.UtcNow;
            session.EndUsedSpace = endUsedSpace;

            await context.SaveChangesAsync();
        }

        public async Task<List<UsbDeviceSessionEntity>> GetOpenSessionsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Session
                .Where(s => s.EndTimestamp == null)
                .ToListAsync();
        }
    }
}
