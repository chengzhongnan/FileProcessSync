using ProjectCommon.Unit;
using System;
using System.Threading.Tasks;
using FileProcessSync.Config;
using FileProcessSync.Handler;

namespace FileProcessSync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 先关闭服务器
            await FileHelper.ProcessCommand("stop");
            
            foreach(var workDir in Config.SyncDirectoryConfig.Instance.WorkDirConfigs)
            {
                var serverData = await FileHelper.GetServerMd5Data(workDir.BaseUrl, workDir.Name);

                var findServerDir = serverData.data.Find(x => x.DirName == workDir.Name);
                if (findServerDir != null)
                {
                    var (different, serverOnly) = FileHelper.CompareWithServerMD5(workDir, findServerDir);
                    foreach (var file in different)
                    {
                        Console.WriteLine("正在同步：" + file.FileName);
                        await FileHelper.PostWorkFile(workDir, file);
                    }
                }
            }

            // 最后再启动服务器
            await FileHelper.ProcessCommand("start");
        }
    }
}