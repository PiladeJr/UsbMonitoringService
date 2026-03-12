using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence.Repositories.Event
{
    public class UsbEventRepository(IDbContextFactory<UsbMonitoringDbContext> contextFactory) : IUsbEventRepository
    {
        private readonly IDbContextFactory<UsbMonitoringDbContext> _contextFactory = contextFactory;

        public async Task SaveEventAsync(UsbDeviceEventEntity evt)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // retry logic for SQLITE_BUSY
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    context.Events.Add(evt);
                    await context.SaveChangesAsync();
                    return;
                }
                catch (SqliteException ex) when (ex.SqliteErrorCode == 5)
                {
                    await Task.Delay(50);
                }
            }
        }

        public async Task<UsbDeviceEventEntity?> GetLastEventAsync(string deviceId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await (from e in context.Events
                          join s in context.Session on e.SessionId equals s.Id
                          where s.DeviceId == deviceId
                          orderby e.Timestamp descending
                          select e)
                .FirstOrDefaultAsync();
        }

        public async Task<UsbDeviceEventEntity?> GetLastEventBySessionIdAsync(Guid sessionId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Events
                .Where(e => e.SessionId == sessionId)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
