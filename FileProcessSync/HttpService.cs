using HttpServerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessSync
{
    class HttpService
    {
        public HttpService()
        {
            _httpServers = new List<HttpServerHandlerManager>();
        }

        List<HttpServerHandlerManager> _httpServers { get; set; }

        public static List<IPAddress> GetAllDnsHost()
        {
            List<IPAddress> ipAddress = new List<IPAddress>();
            ipAddress.Add(IPAddress.Parse("0.0.0.0"));
            ipAddress.Add(IPAddress.Parse("::0"));
            return ipAddress;
        }


        private void CreateHttpServerInstance(IPAddress ip, int port, bool isSecert = false)
        {
            var http = new HttpServerHandlerManager(this);

            http.CreateInstance(ip, port);
            http.RegisterHandler(Assembly.GetEntryAssembly());
            http.Log = Log.Instance.Logger;

            http.CacheFile = false;

            _httpServers.Add(http);
        }

        public void RegisterServer()
        {
            if (Config.HttpServiceConfig.Instance.Host == "Any")//所有IP
            {
                var list = GetAllDnsHost();
                foreach (var it in list)
                {
                    CreateHttpServerInstance(it, Config.HttpServiceConfig.Instance.Port);
                }
            }
            else
            {
                var ip = IPAddress.Parse(Config.HttpServiceConfig.Instance.Host);
                CreateHttpServerInstance(ip, Config.HttpServiceConfig.Instance.Port, false);
            }
        }

        public void UnRegisterServer()
        {
            foreach (var http in _httpServers)
            {
                http.Release();
            }

            _httpServers.Clear();
        }
    }

}
