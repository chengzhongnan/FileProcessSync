using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectCommon.Unit;

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace FileProcessSync.Handler
{
    [HttpServerLib.HttpGet]
    internal class GetWorkDirMd5Handler : HttpServerLib.HttpServerHandlerBase
    {
        public override string Raw => "/sync/api/md5";

        [HttpServerLib.HttpField("dir")]
        public string WorkDir { get; set; }

        class Response
        {
            public string state { get; set; }
            public List<ResponseData> data { get; set; } = new List<ResponseData>();
        }

        class ResponseData
        {
            public string DirName { get; set; }

            public List<FileMD5Info> files { get; set; } = new List<FileMD5Info>();
        }

        class FileMD5Info
        {
            public string FileName { get; set; }
            public string Hash { get; set; }
        }

        private List<FileMD5Info> GetConfigFileMD5Info(Config.WorkDirConfig workDir, string basePath = "/")
        {
            return FileHelper.DoWorkDirWithConfig<FileMD5Info>(workDir, basePath, (currentWorkDir, currentBasePath, fullFileName) =>
            {
                var fileContent = File.ReadAllBytes(fullFileName);
                var fileName = Path.GetFileName(fullFileName);
                var md5 = StaticExtension.MD5(fileContent);

                FileMD5Info md5Info = new FileMD5Info()
                {
                    FileName = currentBasePath + fileName,
                    Hash = md5
                };
                return md5Info;
            });
        }
        

        public override Task<string> Exec(object state)
        {
            Response response = new Response();

            foreach(var config in Config.SyncDirectoryConfig.Instance.WorkDirConfigs)
            {
                if (string.IsNullOrEmpty(WorkDir) || WorkDir == config.Name)
                {
                    ResponseData data = new ResponseData() { DirName = config.Name };
                    data.files.AddRange(GetConfigFileMD5Info(config));
                    response.data.Add(data);
                }
            }

            return Task.FromResult(JsonConvert.SerializeObject(response));
        }
    }
}
