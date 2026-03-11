using UsbMonitoringService;
using UsbMonitoringService.Infrastructure.DeviceInfo;
using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Infrastructure.UsbDetection;
using UsbMonitoringService.Monitor;

var builder = Host.CreateApplicationBuilder(args);
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

await host.RunAsync();
