namespace UsbMonitoringService.Infrastructure.Storage
{
    using System.Collections.Concurrent;
    using UsbMonitoringService.Models;

    public class DeviceRegistry : IDeviceRegistry
    {
        private readonly ConcurrentDictionary<string, UsbDeviceState> _devices = new();

        public void RegisterDevice(UsbDevice device)
        {
            var state = new UsbDeviceState
            {
                Device = device,
                SessionStartUsedSpace = device.UsedSpace,
                LastObservedUsedSpace = device.UsedSpace
            };

            _devices[device.Id] = state;
        }

        public void RemoveDevice(string deviceId)
        {
            _devices.TryRemove(deviceId, out _);
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
    }
}
