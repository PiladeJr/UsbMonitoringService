using UsbMonitoringService.Infrastructure.DriveProvider;

namespace UsbMonitoringService.Monitor
{
    public static class DriveUsageProvider
    {
        public static long GetUsedSpace(string mountPoint)
        {
            var drive = DriveInfoProvider.GetReadyDrive(mountPoint);

            if (drive == null)
                return -1;

            return drive.TotalSize - drive.TotalFreeSpace;
        }
    }
}
