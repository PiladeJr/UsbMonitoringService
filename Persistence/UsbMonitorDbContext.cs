using Microsoft.EntityFrameworkCore;
using UsbMonitoringService.Persistence.Entities;

namespace UsbMonitoringService.Persistence
{
    public class UsbMonitoringDbContext(DbContextOptions<UsbMonitoringDbContext> options) : DbContext(options)
    {
        public DbSet<UsbDeviceEntity> Devices { get; set; }

        public DbSet<UsbDeviceEventEntity> Events { get; set; }

        public DbSet<UsbDeviceSessionEntity> Session { get; set; }
    }
}
