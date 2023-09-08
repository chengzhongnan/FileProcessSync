using ProjectCommon.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace FileProcessSync.Config
{
    class HttpServiceConfig : SingleInstance<HttpServiceConfig>, IConfigBase
    {
        public HttpServiceConfig() => Init();

        private void Init()
        {
            var xml = XElement.Load(IConfigBase.ConfigFileName);

            var cfg = xml.Element("Server").Element("http");
            Host = cfg.Attribute("host").Value;
            Port = int.Parse(cfg.Attribute("port").Value);
        }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}
