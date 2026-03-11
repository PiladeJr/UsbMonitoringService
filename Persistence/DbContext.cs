namespace UsbMonitoringService.Persistence
{
    internal static class DbContext
    {
        public static readonly string ConnectionString =
        $"Data Source={Path.Combine(AppContext.BaseDirectory, "ExternalDevices.db")}";
    }
}
