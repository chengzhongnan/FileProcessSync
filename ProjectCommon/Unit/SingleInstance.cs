using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCommon.Unit
{
    /// <summary>
    /// 单实例
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class SingleInstance<TClass> where TClass: class, new()
    {
        static TClass _instance { get; set; }

        static object _lock = new object();

        public SingleInstance()
        {

        }

        public static TClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TClass();
                        }
                    }                                       
                }

                return _instance;
            }
        }
    }
}
