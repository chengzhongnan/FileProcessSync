using ProjectCommon.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
