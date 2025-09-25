namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Einfaches Logger-Interface für Protokollierungszwecke.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Protokolliert eine Nachricht.
        /// </summary>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        void Log(string message);
    }
}
