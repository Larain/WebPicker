using System;
using System.Threading;
using System.Threading.Tasks;
using WebPicker.Data;
using WebPicker.Models;

namespace WebPicker.Helpers
{
    public class CustomLogger : ILogger, IDisposable
    {
        private readonly ManualResetEvent _workeResetEvent = new ManualResetEvent(false);
        public string Name { get; set; }
        public LoggingDbContext LoggerDbContext { get; }

        public CustomLogger(LoggingDbContext loggerDbContext)
        {
            LoggerDbContext = loggerDbContext;
            StartWorker();
        }

        public void Log(Log log)
        {
            LoggerDbContext.Log.Add(log);
        }

        public void Log(string level, string user, string action, string message, string exception = null)
        {
            var log = new Log
            {
                Date = DateTime.Now,
                Thread = Thread.CurrentThread.ManagedThreadId.ToString(),
                Level = level,
                Logger = Name,
                User = user,
                Action = action,
                Message = message,
                Exception = exception
            };

            LoggerDbContext.Log.Add(log);
        }
        public async void LogAsync(Log log)
        {
            await Task.Factory.StartNew(() =>
            {
                Log(log);
            });
        }

        public async void LogAsync(string level, string user, string action, string message, string exception = null)
        {
            await Task.Factory.StartNew(() =>
            {
                Log(level, user, action, message, exception);
            });
        }

        private void StartWorker()
        {
            _workeResetEvent.Reset();
            Task.Factory.StartNew(DoWork);
        }

        private void DoWork()
        {
            while (!_workeResetEvent.WaitOne(5000))
            {
                if (LoggerDbContext.ChangeTracker.HasChanges())
                    LoggerDbContext.SaveChanges(true);
            }
        }

        public void Dispose()
        {
            _workeResetEvent.Set();
            _workeResetEvent?.Dispose();
            LoggerDbContext?.Dispose();
        }
    }
}
