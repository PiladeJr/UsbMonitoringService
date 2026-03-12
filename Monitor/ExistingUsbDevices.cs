using System.Runtime.Versioning;
using UsbMonitoringService.Infrastructure.DeviceInfo;
using UsbMonitoringService.Infrastructure.DriveProvider;
using UsbMonitoringService.Infrastructure.Storage;
using UsbMonitoringService.Models;
using UsbMonitoringService.Persistence.Entities;
using UsbMonitoringService.Persistence.Repositories.DeviceInfo;
using UsbMonitoringService.Persistence.Repositories.DeviceSession;
using UsbMonitoringService.Persistence.Repositories.Event;

namespace UsbMonitoringService.Monitor
{
    [SupportedOSPlatform("windows")]
    public class ExistingUsbDevices(
        IDeviceInfoProvider deviceInfoProvider,
        IDeviceRegistry deviceRegistry,
        IUsbDeviceRepository deviceRepository,
        IUsbSessionRepository sessionRepository,
        IUsbEventRepository eventRepository,
        ILogger<ExistingUsbDevices> logger)
    {
        private readonly IDeviceInfoProvider _deviceInfoProvider = deviceInfoProvider;
        private readonly IDeviceRegistry _deviceRegistry = deviceRegistry;
        private readonly ILogger<ExistingUsbDevices> _logger = logger;
        private readonly IUsbDeviceRepository _deviceRepo = deviceRepository;
        private readonly IUsbSessionRepository _sessionRepo = sessionRepository;
        private readonly IUsbEventRepository _eventRepo = eventRepository;

        public async Task InitializeDevicesAsync()
        {
            var usbDrives = _deviceInfoProvider.GetUsbDrives();

            var openSessions = await _sessionRepo.GetOpenSessionsAsync();

            var connectedDevices = usbDrives
                .Select(d => d.Disk.DeviceId)
                .ToHashSet();

            var openSessionDevices = openSessions
                .Select(s => s.DeviceId)
                .ToHashSet();

            await RestorePreviousSessionsAsync(openSessions, connectedDevices);

            await RegisterStartupDevicesAsync(usbDrives, openSessionDevices);
        }

        private async Task RestorePreviousSessionsAsync(
            IEnumerable<UsbDeviceSessionEntity> openSessions,
            HashSet<string> connectedDevices)
        {
            foreach (var session in openSessions)
            {
                var device = await _deviceRepo.GetByDeviceIdAsync(session.DeviceId);
                if (device == null)
                    continue;

                if (!connectedDevices.Contains(session.DeviceId))
                {
                    await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
                    {
                        SessionId = session.Id,
                        EventType = "Disconnect",
                        Timestamp = DateTime.UtcNow,
                        Dimension = 0
                    });

                    await _sessionRepo.CloseSessionAsync(
                        session.Id,
                        session.StartUsedSpace);

                    _logger.LogWarning(
                        "The following device has not been detected since the last shutdown: {Name} | DeviceId: {DeviceId} | Mount: {MountPort}",
                        device.Name,
                        session.DeviceId,
                        device.MountPort);

                    continue;
                }

                var usbDevice = new UsbDevice
                {
                    Id = device.Id,
                    Name = device.Name,
                    FabricName = device.FabricName,
                    SerialNumber = device.SerialNumber,
                    MountPoint = device.MountPort
                };

                await _deviceRegistry.RestoreDeviceAsync(
                    usbDevice,
                    session.Id,
                    session.StartUsedSpace);

                _logger.LogInformation(
                    "USB device restored from previous session: {Name} | DeviceId: {DeviceId} | Mount: {MountPort}",
                    device.Name,
                    session.DeviceId,
                    device.MountPort);
            }
        }

        private async Task RegisterStartupDevicesAsync(
            IEnumerable<(string driveLetter, DiskDriveInfo Disk)> usbDrives,
            HashSet<string> openSessionDevices)
        {
            foreach (var (driveLetter, diskInfo) in usbDrives)
            {
                if (openSessionDevices.Contains(diskInfo.DeviceId))
                    continue;

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

                await _deviceRegistry.RegisterDeviceAsync(device);

                _logger.LogWarning(
                    "USB already connected at service startup: {Name} | Serial: {Serial} | Mount: {Mount}",
                    device.Name,
                    device.SerialNumber,
                    device.MountPoint);
            }
        }
    }
}