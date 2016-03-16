namespace CLI.Shared
{
    /// <summary>
    /// Wrap log4net usage in a different class for avoiding others references in the modules
    /// </summary>
    public interface ICliLogger
    {
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as debug
        /// </summary>
        /// <param name="message">String to write in the log</param>
        void Debug(string message);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as debug
        /// </summary>
        /// <param name="message">String to write in the log</param>
        /// <param name="args"></param>
        void DebugFormat(string message, params object[] args);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as informational
        /// </summary>
        /// <param name="message">String to write in the log</param>
        void Info(string message);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as informational
        /// </summary>
        /// <param name="message">String to write in the log</param>
        /// <param name="args"></param>
        void InfoFormat(string message, params object[] args);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as warning
        /// </summary>
        /// <param name="message">String to write in the log</param>
        void Warn(string message);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as warning
        /// </summary>
        /// <param name="message">String to write in the log</param>
        /// <param name="args"></param>
        void WarnFormat(string message, params object[] args);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as error
        /// </summary>
        /// <param name="message">String to write in the log</param>
        void Error(string message);
        /// <summary>
        /// Log the string passed by <paramref name="message"/> as error
        /// </summary>
        /// <param name="message">String to write in the log</param>
        /// <param name="args"></param>
        void ErrorFormat(string message, params object[] args);
    }
}
