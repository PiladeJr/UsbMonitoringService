using UsbMonitoringService.Infrastructure.DeviceInfo;
using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Infrastructure.UsbDetection;
using UsbMonitoringService.Models;
using UsbMonitoringService.Monitor;

namespace UsbMonitoringService
{
    public class Worker(
        ILogger<Worker> logger,
        IUsbEventListener usbListener,
        IDeviceInfoProvider deviceInfoProvider,
        IDeviceRegistry deviceRegistry,
        UsbUsageMonitor usageMonitor,
        ExistingUsbDevices existingUsbDevices) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IUsbEventListener _usbListener = usbListener;
        private readonly IDeviceInfoProvider _deviceInfoProvider = deviceInfoProvider;
        private readonly IDeviceRegistry _deviceRegistry = deviceRegistry;
        private readonly UsbUsageMonitor _usageMonitor = usageMonitor;
        private readonly ExistingUsbDevices _existingUsbDevices = existingUsbDevices;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("USB Monitoring Service started");

            try
            {
                if (OperatingSystem.IsWindows())
                {
                    _existingUsbDevices.LoadExistingDevices(); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading existing USB devices at startup");
            }

            _usbListener.UsbInserted += OnUsbInserted;
            _usbListener.UsbRemoved += OnUsbRemoved;

            _usbListener.Start();
            try
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    _usageMonitor.MonitorDevices();

                    await Task.Delay(1000, stoppingToken); 
                }
            }

            finally
            {
                _logger.LogInformation("Stopping USB listener");
                _usbListener.Stop();
            }
        }

        private async void OnUsbInserted(string driveLetter)
        {
            try
            {
                _logger.LogInformation("USB inserted on drive {Drive}", driveLetter);

                UsbDevice? device = null;

                for (int i = 0; i < 3 && device == null; i++)
                {
                    device = _deviceInfoProvider.GetDeviceInfo(driveLetter);

                    if (device == null)
                        await Task.Delay(500);
                }

                if (device == null)
                {
                    _logger.LogWarning("Device info not resolved for {Drive}", driveLetter);
                    return;
                }

                _deviceRegistry.RegisterDevice(device);

                _logger.LogInformation(
                    "USB device registered: {Name} | Original Name: {FabricName} | Serial: {Serial} | Mount: {Mount}",
                    device.Name,
                    device.FabricName,
                    device.SerialNumber,
                    device.MountPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling USB insertion");
            }
        }

        private void OnUsbRemoved(string driveLetter)
        {
            try
            {
                _logger.LogInformation("USB removed from drive {Drive}", driveLetter);

                var devices = _deviceRegistry.GetAllDevices();

                var device = devices.FirstOrDefault(d =>
                    string.Equals(d.Device.MountPoint, driveLetter + "\\", StringComparison.OrdinalIgnoreCase));

                if (device != null)
                {
                    _deviceRegistry.RemoveDevice(device.Device.Id);

                    _logger.LogInformation("Device removed from registry: {Device}", device.Device.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling USB removal");
            }
        }
    }
}
