namespace UsbMonitoringService.Models
{
    public class UsbDeviceState
    {
        /// <summary>
        /// Composizione dell'oggetto <see cref="UsbDevice">UsbDevice </see> associato a questo stato. 
        /// Può essere null se il dispositivo è stato rimosso.
        /// </summary>
        public UsbDevice? Device { get; set; }
        /// <summary>
        /// L'id della sessione di traccia associata a questo dispositivo.
        /// </summary>
        public Guid SessionId { get; set; }
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
        /// <summary>
        /// Il numero totale di byte in attesa di essere scritti sul db.
        /// </summary>
        /// <remarks>
        /// Parametro di utilità per il database. permette di tenere traccia della quantità di byte
        /// che vengono scritti sul dispositivo. Questo parametro verrà persistito nel momento in cui
        /// non rilevo ulteriori operazioni di scrittura. Per saperlo sfrutto il parametro <see cref="LastWriteTimestamp">LastWriteTimestamp</see>
        /// </remarks>
        public long PendingWriteBytes { get; set; }
        /// <summary>
        /// Timestamp dell'ultima scrittura osservata sul dispositivo.
        /// </summary>
        /// <remarks>
        /// Parametro di utilità per il database. permette di tenere traccia del momento in cui 
        /// è stata osservata l'ultima scrittura, in modo da confrontarla con 
        /// <c>DateTime.UtcNow</c>. Se la differenza è maggiore di una certa soglia,
        /// vuol dire che il dispositivo ha terminato l'operazione di scrittura e l'evento è
        /// quindi persistibile sul db.
        /// </remarks>
        public DateTime LastWriteTimestamp { get; set; }
    }
}
