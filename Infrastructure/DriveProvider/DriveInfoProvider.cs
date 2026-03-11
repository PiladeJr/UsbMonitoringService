namespace UsbMonitoringService.Infrastructure.DriveProvider
{
    public static class DriveInfoProvider
    {
        public static DriveInfo? GetReadyDrive(string mountPoint)
        {
            if (string.IsNullOrWhiteSpace(mountPoint))
                return null;

            var drive = new DriveInfo(mountPoint);

            if (!drive.IsReady)
                return null;

            return drive;
        }
    }
}
