using ProjectCommon.Unit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。

namespace FileProcessSync.Config
{
    class SyncDirectoryConfig : SingleInstance<SyncDirectoryConfig>, IConfigBase
    {
        public SyncDirectoryConfig() 
        { 
            Init();
        }

        private void Init()
        {
            var xml = XElement.Load(IConfigBase.ConfigFileName);
            var cfg = xml.Element("Server").Element("Directory");

            foreach (var xSub in cfg.Elements("dir"))
            {
                WorkDirConfig workDirConfig = new WorkDirConfig();
                workDirConfig.Load(xSub);
                WorkDirConfigs.Add(workDirConfig);
            }
        }

        public List<WorkDirConfig> WorkDirConfigs { get; set; } = new List<WorkDirConfig>();
    }

    class WorkDirConfig
    {
        public void Load(XElement xEle)
        {
            Name = xEle.Attribute("name").Value;
            IncludeSubDir = bool.Parse(xEle.Attribute("includeSubDir").Value);
            Path = xEle.Attribute("path").Value;
            foreach (var xSub in xEle.Elements("match"))
            {
                Matches.Add(xSub.Value);
            }
            foreach (var xSub in xEle.Elements("exclude"))
            {
                Excludes.Add(xSub.Value);
            }
        }

        public bool IsMatch(string fileName)
        {
            // 先匹配排除项，如果排除了，直接返回不匹配
            foreach (var filter in RegexExcludes)
            {
                if (filter.IsMatch(fileName))
                {
                    return false;
                }
            }

            // 再匹配
            foreach (var filter in RegexMatches)
            {
                if (filter.IsMatch(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        public WorkDirConfig Clone()
        {
            WorkDirConfig workDir = new WorkDirConfig()
            {
                Name = Name,
                IncludeSubDir = IncludeSubDir,
                Path = Path,
                Excludes = Excludes,
                Matches = Matches,
            };

            return workDir;
        }

        public string Name { get; set; }
        public bool IncludeSubDir { get; set; }
        public string Path { get; set; }

        private List<Regex> _regexMatches = null;
        private List<Regex> RegexMatches  
        {
            get
            {
                if (_regexMatches == null)
                {
                    _regexMatches = new List<Regex>();
                    foreach (var filter in Matches)
                    {
                        try
                        {
                            _regexMatches.Add(new Regex(filter));
                        }
                        catch { }
                    }
                }

                return _regexMatches;
            }
        }

        private List<Regex> _regexExcludes = null;
        private List<Regex> RegexExcludes
        {
            get
            {
                if (_regexExcludes == null)
                {
                    _regexExcludes = new List<Regex>();
                    foreach (var filter in Excludes)
                    {
                        try
                        {
                            _regexExcludes.Add(new Regex(filter));
                        }
                        catch { }
                    }
                }

                return _regexExcludes;
            }
        }

        public List<string> Matches { get; set; } = new List<string>();
        public List<string> Excludes { get; set; } = new List<string> { };
    }
}
