namespace UsbMonitoringService.Infrastructure.UsbDetection
{
    public interface IUsbEventListener
    {
        event Action<string> UsbInserted;

        event Action<string> UsbRemoved;

        void Start();

        void Stop();
    }
}
