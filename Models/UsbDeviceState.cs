namespace UsbMonitoringService.Models
{
    public class UsbDeviceState
    {
        public UsbDevice? Device { get; set; }
        /// <summary>
        /// La dimensione di partenza del dispositivo al momento dell'inserimento.
        /// </summary>
        public long SessionStartUsedSpace { get; set; }
        /// <summary>
        /// L'ultimo valore osservato dello spazio usato sul dispositivo.
        /// </summary>
        public long LastObservedUsedSpace { get; set; }
        /// <summary>
        /// Lo spazio utilizzato dal dispositivo al momento dell'ultima osservazione.
        /// </summary>
        public long CurrentUsedSpace { get; set; }
    }
}
