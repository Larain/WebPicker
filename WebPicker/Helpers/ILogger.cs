using WebPicker.Models;

namespace WebPicker.Helpers
{
    public interface ILogger
    {
        void Log(Log log);
        void LogAsync(Log log);
        void Log(string level, string user, string action, string message, string exception = null);
        void LogAsync(string level, string user, string action, string message, string exception = null);
    }
}