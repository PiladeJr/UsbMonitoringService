using System.Runtime.Versioning;
using UsbMonitoringService.Infrastructure.DeviceInfo;
using UsbMonitoringService.Infrastructure.DriveProvider;
using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Models;

namespace UsbMonitoringService.Monitor
{
    [SupportedOSPlatform("windows")]
    public class ExistingUsbDevices(
        IDeviceInfoProvider deviceInfoProvider,
        IDeviceRegistry deviceRegistry,
        ILogger<ExistingUsbDevices> logger)
    {
        private readonly IDeviceInfoProvider _deviceInfoProvider = deviceInfoProvider;
        private readonly IDeviceRegistry _deviceRegistry = deviceRegistry;
        private readonly ILogger<ExistingUsbDevices> _logger = logger;

        public void LoadExistingDevices()
        {
            var usbDrives = _deviceInfoProvider.GetUsbDrives();

            foreach (var (driveLetter, diskInfo) in usbDrives)
            {
                var drive = DriveInfoProvider.GetReadyDrive(driveLetter);

                if (drive == null)
                    continue;

                var device = new UsbDevice
                {
                    Id = diskInfo.DeviceId,
                    Name = drive.VolumeLabel,
                    FabricName = diskInfo.Model,
                    SerialNumber = diskInfo.SerialNumber,
                    MountPoint = drive.Name,
                    TotalSpace = drive.TotalSize,
                    UsedSpace = drive.TotalSize - drive.TotalFreeSpace,
                    InsertTimestamp = DateTime.UtcNow
                };

                _deviceRegistry.RegisterDevice(device);

                _logger.LogInformation(
                    "Already connected USB device detected: {Name} | Serial: {Serial} | Mount: {Mount}",
                    device.Name,
                    device.SerialNumber,
                    device.MountPoint);
            }
        }
    }
}