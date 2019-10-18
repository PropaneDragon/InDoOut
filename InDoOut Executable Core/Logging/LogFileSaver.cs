using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Location;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace InDoOut_Executable_Core.Logging
{
    public class LogFileSaver
    {
        private static readonly int COMPLETE_REFRESH_AFTER_COUNTER = 60;

        private readonly IStandardLocations _locations = null;
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds) { Enabled = false };
        private int _completeResetTime = 0;
        private DateTime _lastLogTime = DateTime.MinValue;

        public string LogFileName => "IDO.log";

        public LogFileSaver(IStandardLocations locations)
        {
            _locations = locations;
            _timer.Elapsed += Timer_Elapsed;

            _ = ClearLog();
        }

        public void BeginAutoSave()
        {
            _timer.Start();
        }

        public void StopAutoSave()
        {
            _timer.Stop();
        }

        public bool ClearLog()
        {
            var applicationDirectory = _locations?.GetPathTo(Location.Location.ApplicationDirectory);

            return ClearLog(applicationDirectory);
        }

        public bool SaveLog()
        {
            var applicationDirectory = _locations?.GetPathTo(Location.Location.ApplicationDirectory);

            return SaveLog(applicationDirectory);
        }

        private bool ClearLog(string location)
        {
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location))
            {
                try
                {
                    var fullPath = Path.Combine(location, LogFileName);
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        if (File.Exists(fullPath))
                        {
                            File.WriteAllText(fullPath, "");

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error($"Couldn't clear log file due to an error: {ex.Message}");
                }
            }

            return false;
        }

        private bool SaveLog(string location)
        {
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location))
            {
                try
                {
                    var fullPath = Path.Combine(location, LogFileName);
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        var recentLogs = Log.Instance.Logs.Where(log => log.Time > _lastLogTime);
                        if (recentLogs.Count() > 0)
                        {
                            var fullLogString = string.Join("\n", recentLogs) + "\n";

                            File.AppendAllText(fullPath, fullLogString);

                            _lastLogTime = recentLogs?.LastOrDefault()?.Time ?? DateTime.MinValue;
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error($"Couldn't save log file due to an error: {ex.Message}");
                }
            }

            return false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            if (++_completeResetTime > COMPLETE_REFRESH_AFTER_COUNTER && ClearLog() && SaveLog())
            {
                _completeResetTime = 0;
                _lastLogTime = DateTime.MinValue;
            }
            else
            {
                _ = SaveLog();
            }

            _timer.Start();
        }
    }
}
