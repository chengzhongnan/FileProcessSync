using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8604 // 引用类型参数可能为 null。

namespace FileProcessSync.Handler
{
    [HttpServerLib.HttpPost]
    internal class SyncFileHandler : HttpServerLib.HttpServerHandlerBase
    {
        public override string Raw => "/sync/api/file";

        class SyncFileInfo
        {
            public string SyncName { get; set; }
            public string SyncFile { get; set; }
            
            public string FileDataBase64 { get; set; }
            public byte[] FileData => Convert.FromBase64String(FileDataBase64);
        }

        class Response
        {
            public string state { get; set; }
        }

        private static void CreateDirectory(string path)
        {
            path = Path.GetFullPath(path);

            // 拆分目录路径为各级目录
            string[] directories = path.Split('/');

            string currentPath = "";

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                currentPath = "/";
            }

            foreach (string directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory))
                {
                    continue;
                }
                currentPath = Path.Combine(currentPath, directory);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        public override Task<string> Exec(object state)
        {
            Response resp = new Response();
            var json = Encoding.UTF8.GetString(PostData);

            SyncFileInfo syncInfo = JsonConvert.DeserializeObject<SyncFileInfo>(json);

            var config = Config.SyncDirectoryConfig.Instance.WorkDirConfigs.Find(x => x.Name == syncInfo.SyncName);
            if (!Directory.Exists(config.Path))
            {
                var path = Path.GetFullPath(config.Path);
                CreateDirectory(path);
            }
            if (config != null)
            {
                var file = config.Path + syncInfo.SyncFile;
                var dir = Path.GetFullPath(file);
                dir = Path.GetDirectoryName(dir);
                if (!Directory.Exists(dir))
                {
                    CreateDirectory(dir);
                }
                Log.Debug($"正在更新文件：{file}");
                System.IO.File.WriteAllBytes(file, syncInfo.FileData);
            }
            else
            {
                resp.state = "invalid sync name";
            }

            return Task.FromResult(JsonConvert.SerializeObject(resp));
        }
    }
}
