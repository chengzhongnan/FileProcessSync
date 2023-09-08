using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessSync.Config
{
    interface IConfigBase
    {
        protected static string ConfigFileName => "App.Config";
    }
}
