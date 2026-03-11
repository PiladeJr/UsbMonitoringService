using System.IO;
using System.Management;
using System.Runtime.Versioning;
using UsbMonitoringService.Infrastructure.DriveProvider;
using UsbMonitoringService.Models;

namespace UsbMonitoringService.Infrastructure.DeviceInfo
{
    [SupportedOSPlatform("windows")]
    public class UsbDeviceInfoProvider : IDeviceInfoProvider
    {
        public UsbDevice? GetDeviceInfo(string driveLetter)
        {
            var driveInfo = DriveInfoProvider.GetReadyDrive(driveLetter);

            if (driveInfo == null)
                return null;

            var deviceInfo = GetDiskDriveInfo(driveInfo.Name);

            if (deviceInfo == null)
                return null;

            return new UsbDevice
            {
                Id = deviceInfo.DeviceId,
                Name = string.IsNullOrWhiteSpace(driveInfo.VolumeLabel)
                    ? deviceInfo.Model
                    : driveInfo.VolumeLabel,
                FabricName = deviceInfo.Model,
                SerialNumber = deviceInfo.SerialNumber,
                MountPoint = driveInfo.Name,
                TotalSpace = driveInfo.TotalSize,
                UsedSpace = driveInfo.TotalSize - driveInfo.TotalFreeSpace,
                InsertTimestamp = DateTime.UtcNow
            };
        }

        private static DiskDriveInfo? GetDiskDriveInfo(string driveLetter)
        {
            try
            {
                var logicalDiskId = NormalizeLogicalDiskId(driveLetter);

                if (string.IsNullOrWhiteSpace(logicalDiskId))
                    return null;

                string logicalDiskQuery =
                    $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{logicalDiskId}'}} WHERE AssocClass=Win32_LogicalDiskToPartition";

                using var partitionSearcher = new ManagementObjectSearcher(logicalDiskQuery);

                foreach (ManagementObject partition in partitionSearcher.Get().OfType<ManagementObject>())
                {
                    string? partitionId = partition["DeviceID"]?.ToString();

                    if (string.IsNullOrWhiteSpace(partitionId))
                        continue;

                    string diskQuery =
                        $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition";

                    using var diskSearcher = new ManagementObjectSearcher(diskQuery);

                    foreach (ManagementObject disk in diskSearcher.Get().OfType<ManagementObject>())
                    {
                        var interfaceType = disk["InterfaceType"]?.ToString();

                        if (!string.Equals(interfaceType, "USB", StringComparison.OrdinalIgnoreCase))
                            continue;

                        return ExtractDiskInfo(disk);
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<(string DriveLetter, DiskDriveInfo Disk)> GetUsbDrives()
        {
            var result = new List<(string, DiskDriveInfo)>();

            var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

            foreach (ManagementObject disk in searcher.Get().OfType<ManagementObject>())
            {
                var diskId = disk["DeviceID"]?.ToString();
                if (diskId == null)
                    continue;

                var diskInfo = ExtractDiskInfo(disk);

                var partitionQuery =
                    $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{diskId}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition";

                using var partitionSearcher = new ManagementObjectSearcher(partitionQuery);

                foreach (ManagementObject partition in partitionSearcher.Get().OfType<ManagementObject>())
                {
                    var partitionId = partition["DeviceID"]?.ToString();
                    if (partitionId == null)
                        continue;

                    var logicalQuery =
                        $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} WHERE AssocClass=Win32_LogicalDiskToPartition";

                    using var logicalSearcher = new ManagementObjectSearcher(logicalQuery);

                    foreach (ManagementObject logicalDisk in logicalSearcher.Get().OfType<ManagementObject>())
                    {
                        var driveLetter = logicalDisk["DeviceID"]?.ToString();

                        if (!string.IsNullOrEmpty(driveLetter))
                        {
                            result.Add((driveLetter, diskInfo));
                        }
                    }
                }
            }

            return result;
        }

        private static string? NormalizeLogicalDiskId(string driveLetter)
        {
            if (string.IsNullOrWhiteSpace(driveLetter))
                return null;

            return driveLetter.Trim().TrimEnd('\\');
        }

        private static DiskDriveInfo ExtractDiskInfo(ManagementObject disk)
        {
            var pnpId = disk["PNPDeviceID"]?.ToString();
            string serial = "unknown";

            if (!string.IsNullOrWhiteSpace(pnpId))
            {
                var parts = pnpId.Split('\\');

                if (parts.Length > 2)
                {
                    var serialPart = parts[2];

                    serial = serialPart.Split('&')[0];
                }
            }

            return new DiskDriveInfo
            {
                DeviceId = pnpId ?? string.Empty,
                Model = disk["Model"]?.ToString() ?? string.Empty,
                SerialNumber = serial
            };
        }
    }
}