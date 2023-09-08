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
    class CommandConfig : SingleInstance<CommandConfig>, IConfigBase
    {
        public CommandConfig() 
        {
            Init();
        }

        private void Init()
        {
            var xml = XElement.Load(IConfigBase.ConfigFileName);
            var cfg = xml.Element("Server").Element("Commands");

            foreach (var item in cfg.Elements("cmd"))
            {
                CmdConfig cmd = new CmdConfig();
                cmd.Load(item);

                if (CommandMap.ContainsKey(cmd.Name))
                {
                    CommandMap[cmd.Name].Add(cmd);
                }
                else
                {
                    CommandMap.Add(cmd.Name, new List<CmdConfig>() { cmd });
                }
            }
        }

        public Dictionary<string, List<CmdConfig>> CommandMap { get; set; } = new Dictionary<string, List<CmdConfig>>();
    }

    class CmdConfig
    {
        public void Load(XElement xEle)
        {
            Name = xEle.Attribute("name").Value;
            FileName = xEle.Attribute("file").Value;
            Argument = xEle.Attribute("args").Value;
            WorkDir = xEle.Attribute("workdir").Value;
        }

        public string Name { get; set; }
        public string FileName { get; set; }
        public string Argument { get; set; }
        public string WorkDir { get; set; }
    }
}
