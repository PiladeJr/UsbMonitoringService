using Microsoft.EntityFrameworkCore;
using UsbMonitoringService;
using UsbMonitoringService.Infrastructure.DeviceInfo;
using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Infrastructure.UsbDetection;
using UsbMonitoringService.Monitor;
using UsbMonitoringService.Persistence;
using UsbMonitoringService.Persistence.Repositories.DeviceInfo;
using UsbMonitoringService.Persistence.Repositories.DeviceSession;
using UsbMonitoringService.Persistence.Repositories.Event;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<UsbMonitoringDbContext>(
    options => options.UseSqlite("Data Source=usb-monitor.db"),
    ServiceLifetime.Singleton);
builder.Services.AddSingleton<IUsbEventRepository, UsbEventRepository>();
builder.Services.AddSingleton<IUsbDeviceRepository, UsbDeviceRepository>();
builder.Services.AddSingleton<IUsbSessionRepository, UsbSessionRepository>();

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IUsbEventListener, WmiUsbEventListener>();
    builder.Services.AddSingleton<IDeviceInfoProvider, UsbDeviceInfoProvider>();
    builder.Services.AddSingleton<ExistingUsbDevices>();
}

builder.Services.AddSingleton<IDeviceRegistry, DeviceRegistry>();
builder.Services.AddSingleton<UsbUsageMonitor>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UsbMonitoringDbContext>();
    await context.Database.EnsureCreatedAsync();
}

await host.RunAsync();
