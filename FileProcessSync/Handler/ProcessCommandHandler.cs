using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace FileProcessSync.Handler
{
    [HttpServerLib.HttpGet]
    internal class ProcessCommandHandler : HttpServerLib.HttpServerHandlerBase
    {
        public override string Raw => "/sync/api/cmd";

        [HttpServerLib.HttpField("cmd")]
        public string Cmd { get; set; }

        class Response
        {
            public string status { get; set; }
        }

        public override Task<string> Exec(object state)
        {
            Response response = new Response();
            var cmdMap = Config.CommandConfig.Instance.CommandMap;

            Log.Debug($"执行命令：{Cmd}");

            if (cmdMap.ContainsKey(Cmd))
            {
                foreach(var config in cmdMap[Cmd])
                {
                    Log.Debug($"正在执行命令：{config.FileName} {config.Argument}");
                    Process process = new Process();
                    process.StartInfo.WorkingDirectory = config.WorkDir;
                    process.StartInfo.FileName = config.FileName;
                    process.StartInfo.Arguments = config.Argument;
                    process.StartInfo.UseShellExecute = true;

                    process.Start();
                }
            }
            else
            {
                response.status = "Invalid command";
            }

            return Task.FromResult(JsonConvert.SerializeObject(response));
        }
    }
}
