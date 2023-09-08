using Newtonsoft.Json;
using ProjectCommon.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
#pragma warning disable CS8603 // 可能返回 null 引用。

namespace FileProcessSync.Handler
{
    internal static class FileHelper
    {
        /// <summary>
        /// 根据配置遍历文件目录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="workDir"></param>
        /// <param name="basePath"></param>
        /// <param name="funcWork">第一个参数是配置，第二个参数是当前basePath，第三个参数是当前遍历到的文件名（全量）</param>
        /// <returns></returns>
        public static List<T> DoWorkDirWithConfig<T>(Config.WorkDirConfig workDir, string basePath, Func<Config.WorkDirConfig, string, string, T> funcWork)
            where T : class, new()
        {
            List<T> results = new List<T>();

            if (!Directory.Exists(workDir.Path))
            {
                return results;
            }

            var absolutePath = Path.GetFullPath(workDir.Path);
            var files = Directory.GetFiles(absolutePath);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (workDir.IsMatch(fileName))
                {
                    var r = funcWork(workDir, basePath, file);
                    if (r != null)
                    {
                        results.Add(r);
                    }
                }
            }

            if (workDir.IncludeSubDir)
            {
                var subDirs = Directory.GetDirectories(workDir.Path);
                foreach (var subDir in subDirs)
                {
                    var subDirPath = Path.GetFileName(subDir);

                    var subBasePath = basePath + subDirPath + "/";

                    var subWorkDir = workDir.Clone();
                    subWorkDir.Path = subDir + "/";

                    var subFileHashs = DoWorkDirWithConfig(subWorkDir, subBasePath, funcWork);
                    results.AddRange(subFileHashs);
                }
            }

            return results;
        }

        public static List<FileMD5Info> GetConfigFileMD5Info(Config.WorkDirConfig workDir, string basePath = "/")
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

        /// <summary>
        /// 比较当前配置目录里面与服务器不相同的文件
        /// </summary>
        /// <param name="workDir"></param>
        /// <param name="serverData"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static (List<FileMD5Info>, List<FileMD5Info>) CompareWithServerMD5(Config.WorkDirConfig workDir, FileMd5ResponseData serverData, string basePath = "/")
        {
            var currentResult = GetConfigFileMD5Info(workDir, basePath);

            List<FileMD5Info> different = new List<FileMD5Info>();
            foreach(var md5Info in currentResult)
            {
                var find = serverData.files.Find(x => x.FileName == md5Info.FileName);
                if (find == null || find.Hash != md5Info.Hash)
                {
                    different.Add(md5Info);
                }
            }

            List<FileMD5Info> serverOnly = new List<FileMD5Info>();
            foreach (var md5Info in serverData.files)
            {
                var find = currentResult.Find(x => x.FileName == md5Info.FileName);
                if (find == null)
                {
                    serverOnly.Add(md5Info);
                }
            }

            return (different, serverOnly);
        }

        public static async Task<FileMd5Response> GetServerMd5Data(string serverUrl, string name)
        {
            var apiPath = "/sync/api/md5";
            var respData = await StaticExtension.SendRequestGet(serverUrl + apiPath + "?dir=" + name);

            return JsonConvert.DeserializeObject<FileMd5Response>(respData);
        }

        private static List<string> GetAllBaseUrl()
        {
            List<string> baseUrl = new List<string>();
            if (!string.IsNullOrEmpty(Config.ServerConfig.Instance.BaseUrl))
            {
                baseUrl.Add(Config.ServerConfig.Instance.BaseUrl);
            }

            foreach(var work in Config.SyncDirectoryConfig.Instance.WorkDirConfigs)
            {
                if (!string.IsNullOrEmpty(work.BaseUrl))
                {
                    if (!baseUrl.Contains(work.BaseUrl))
                    {
                        baseUrl.Add(work.BaseUrl);
                    }
                }
            }

            return baseUrl;
        }

        public static async Task ProcessCommand(string command)
        {
            var apiPath = "/sync/api/cmd";
            var baseUrlList = GetAllBaseUrl();
            foreach(var url in baseUrlList)
            {
                try
                {
                    await StaticExtension.SendRequestGet(url + apiPath + "?cmd=" + command);
                }
                finally { }
            }
            
        }

        public static async Task<string> PostWorkFile(Config.WorkDirConfig workDir, FileMD5Info file)
        {
            var apiPath = "/sync/api/file";

            var fileName = workDir.Path + file.FileName;
            var fileData = System.IO.File.ReadAllBytes(fileName);

            var syncFileInfo = new SyncFileInfo()
            {
                SyncName = workDir.Name,
                SyncFile = file.FileName,
                FileDataBase64 = Convert.ToBase64String(fileData),
            };

            var postData = JsonConvert.SerializeObject(syncFileInfo);
            if (string.IsNullOrEmpty(workDir.BaseUrl))
            {
                var respData = await StaticExtension.SendRequestPost(Config.ServerConfig.Instance.BaseUrl + apiPath, postData);

                return respData;
            }
            else
            {
                var respData = await StaticExtension.SendRequestPost(workDir.BaseUrl + apiPath, postData);

                return respData;
            }
        }
    }

    class SyncFileInfo
    {
        public string SyncName { get; set; }
        public string SyncFile { get; set; }

        public string FileDataBase64 { get; set; }
        public byte[] FileData => Convert.FromBase64String(FileDataBase64);
    }

    class FileMd5Response
    {
        public string state { get; set; }
        public List<FileMd5ResponseData> data { get; set; } = new List<FileMd5ResponseData>();
    }

    class FileMd5ResponseData
    {
        public string DirName { get; set; }

        public List<FileMD5Info> files { get; set; } = new List<FileMD5Info>();
    }

    class FileMD5Info
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
    }
}
