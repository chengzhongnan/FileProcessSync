using ProjectCommon.Unit;
using System;
using System.Threading.Tasks;
using FileProcessSync.Config;

namespace FileProcessSync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WritePidFile();
            InitDatabases();

            HttpService service = new HttpService();
            service.RegisterServer();

            while (true)
            {
                await Task.Delay(100);
            }

            service.UnRegisterServer();
        }

        private static void InitDatabases()
        {
        }

        private static void WritePidFile()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            System.IO.File.WriteAllText(Config.ServerConfig.Instance.PidFile, process.Id.ToString());
        }
    }

    class Log : SingleInstance<Log>
    {
        log4net.ILog? _Log;
        public log4net.ILog Logger
        {
            get
            {
                if (_Log == null)
                {
                    log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
                    _Log = log4net.LogManager.GetLogger(nameof(Log));
                }

                return _Log;
            }
        }
    }
}
