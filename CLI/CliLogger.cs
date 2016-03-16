using Castle.Core.Logging;
using CLI.Shared;

namespace Cli
{
    public class CliLogger : ICliLogger
    {
        readonly ILogger _logger;
        public CliLogger(ILogger logger)
        {
            _logger = logger;
        }
        public void Debug(string message)
        {
            _logger.Debug(message);
        }
        public void DebugFormat(string message, params object[] args)
        {
            _logger.DebugFormat(message, args);
        }
        public void Info(string message)
        {
            _logger.Info(message);
        }
        public void InfoFormat(string message, params object[] args)
        {
            _logger.InfoFormat(message, args);
        }
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
        public void WarnFormat(string message, params object[] args)
        {
            _logger.WarnFormat(message, args);
        }
        public void Error(string message)
        {
            _logger.Error(message);
        }
        public void ErrorFormat(string message, params object[] args)
        {
            _logger.ErrorFormat(message, args);
        }
    }
}
