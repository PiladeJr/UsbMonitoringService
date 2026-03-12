namespace UsbMonitoringService.Infrastructure.Storage
{
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using UsbMonitoringService.Models;
    using UsbMonitoringService.Persistence.Entities;
    using UsbMonitoringService.Persistence.Repositories.DeviceInfo;
    using UsbMonitoringService.Persistence.Repositories.DeviceSession;
    using UsbMonitoringService.Persistence.Repositories.Event;

    public class DeviceRegistry(
        IUsbSessionRepository sessionRepository,
        IUsbEventRepository eventRepository,
        IUsbDeviceRepository deviceRepository
        ) : IDeviceRegistry
    {
        private readonly ConcurrentDictionary<string, UsbDeviceState> _devices = new();
        private readonly IUsbSessionRepository _sessionRepo = sessionRepository;
        private readonly IUsbEventRepository _eventRepo = eventRepository;
        private readonly IUsbDeviceRepository _deviceRepo = deviceRepository;

        public async Task RegisterDeviceAsync(UsbDevice device)
        {
            if (!_devices.ContainsKey(device.Id))
            {
                if (!await _deviceRepo.ExistsAsync(device.Id))
                {
                    await _deviceRepo.InsertAsync(new UsbDeviceEntity
                    {
                        Id = device.Id,
                        Name = device.Name,
                        FabricName = device.FabricName,
                        SerialNumber = device.SerialNumber,
                        MountPort = device.MountPoint
                    });
                }

                var sessionId = await _sessionRepo.CreateSessionAsync(
                    device.Id,
                    device.UsedSpace);

                var state = new UsbDeviceState
                {
                    Device = device,
                    SessionId = sessionId,
                    LastObservedUsedSpace = device.UsedSpace,
                    PendingWriteBytes = 0,
                    LastWriteTimestamp = DateTime.UtcNow
                };

                // evento di connect
                await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
                {
                    SessionId = state.SessionId,
                    EventType = "Connect",
                    Timestamp = DateTime.UtcNow,
                    Dimension = 0
                });

                _devices[device.Id] = state;
            }
        }

        public async Task RemoveDeviceAsync(string deviceId)
        {
            if (_devices.TryRemove(deviceId, out var state))
            {
                // flush scritture pendenti
                if (state.PendingWriteBytes > 0)
                {
                    await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
                    {
                        SessionId = state.SessionId,
                        EventType = "Write",
                        Timestamp = DateTime.UtcNow,
                        Dimension = state.PendingWriteBytes
                    });
                }

                // evento di disconnect
                await _eventRepo.SaveEventAsync(new UsbDeviceEventEntity
                {
                    SessionId = state.SessionId,
                    EventType = "Disconnect",
                    Timestamp = DateTime.UtcNow,
                    Dimension = 0
                });

                // chiusura sessione
                await _sessionRepo.CloseSessionAsync(
                    state.SessionId,
                    state.LastObservedUsedSpace);
            }
        }

        public IReadOnlyCollection<UsbDeviceState> GetAllDevices()
        {
            return [.. _devices.Values];
        }

        public UsbDeviceState? GetDevice(string deviceId)
        {
            _devices.TryGetValue(deviceId, out var device);

            return device;
        }

        public Task RestoreDeviceAsync(
            UsbDevice device,
            Guid sessionId,
            long lastObservedUsedSpace)
        {
            var state = new UsbDeviceState
            {
                Device = device,
                SessionId = sessionId,
                LastObservedUsedSpace = lastObservedUsedSpace,
                PendingWriteBytes = 0,
                LastWriteTimestamp = DateTime.UtcNow
            };

            _devices[device.Id] = state;

            return Task.CompletedTask;
        }
    }
}
