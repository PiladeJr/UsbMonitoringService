using System.Management;
using UsbMonitoringService.Models;

namespace UsbMonitoringService.Infrastructure.DeviceInfo
{
    public interface IDeviceInfoProvider
    {
        UsbDevice? GetDeviceInfo(string driveLetter);
        IEnumerable<(string DriveLetter, DiskDriveInfo Disk)> GetUsbDrives();
    }
}
