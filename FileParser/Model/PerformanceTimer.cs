using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileParser.Model
{
    public class Logger
    {
        private StringBuilder builder;
        public Logger()
        {
            builder = new StringBuilder();
        }
        public void AppendLine(string message)
        {
            builder.AppendLine($"{DateTime.Now} | {message}");
        }
        public void LogMessage(string message)
        {
            using (StreamWriter sw = new StreamWriter("TimeLog.txt", true))
            {
                sw.WriteLine($"{DateTime.Now} | {message}");
            }
        }
        public void LogBuilder()
        {
            builder.AppendLine(new string('-', 100));
            using (StreamWriter sw = new StreamWriter("TimeLog.txt", true))
            {
                sw.WriteLine(builder.ToString());
            }
            builder.Clear();
        }
    }

    public class PerformanceTimer : IDisposable
    {
        private Logger logger;
        private Stopwatch startTime;
        private string logMessage;

        public PerformanceTimer(Logger logger)
        {
            this.logger = logger;
        }

        public PerformanceTimer(Logger logger, string logMessage) : this (logger)
        {
            Start(logMessage);
        }

        public void Start(string logMessage)
        {
            this.logMessage = logMessage;

            if (startTime != null && startTime.IsRunning)
            {
                throw new Exception("Timer was started");
            }
            logger.AppendLine($"{logMessage} timer strat");
            startTime = Stopwatch.StartNew();
        }

        public void Stop()
        {
            if (startTime != null && startTime.IsRunning)
            {
                startTime.Stop();
                var resultTime = startTime.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                resultTime.Hours,
                resultTime.Minutes,
                resultTime.Seconds,
                resultTime.Milliseconds);

                logger.AppendLine($"{logMessage} timer end {elapsedTime}");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
