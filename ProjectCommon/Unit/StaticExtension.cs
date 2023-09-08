using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Security.Cryptography;

namespace ProjectCommon.Unit
{
    public static class StaticExtension
    {
        /// <summary>
        /// 判断一个集合中是否包含符合某个条件的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> selector )
        {
            foreach(var s in source)
            {
                if(selector(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取得一个集合中包含某个条件的最大的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="bCheckFirst"></param>
        /// <returns></returns>
        public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector, bool bCheckFirst = false)
            where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T maxObj = default(T);
            U maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }
            }
            if (first && bCheckFirst)
                throw new InvalidOperationException("Sequence is empty.");
            return maxObj;
        }

        /// <summary>
        /// 取得一个集合中包含某个条件的最小对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="bCheckFirst"></param>
        /// <returns></returns>
        public static T MinObject<T, U>(this IEnumerable<T> source, Func<T, U> selector, bool bCheckFirst = false)
            where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T minObj = default(T);
            U minKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    minObj = item;
                    minKey = selector(minObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(minKey) < 0)
                    {
                        minKey = currentKey;
                        minObj = item;
                    }
                }
            }
            if (first && bCheckFirst)
                throw new InvalidOperationException("Sequence is empty.");
            return minObj;
        }

        private static Random _random = null;
        private static Random random => _random ?? (_random = new Random(Environment.TickCount));

        public static int RandNext(int min = 0, int max = int.MaxValue)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// 从一个结构体数组中按以下规则随机取得一个数
        /// 结构体中某个字段可以是这样的一系列数，比如a[0]=100,a[1]=200,a[2]=300,a[3]=400
        /// 那么先计算总和为1000，然后从0到1000中随机取得一个数，看位于区间[0,100),[100,300),[300,600),[600,1000)
        /// 中的某一个，则返回对应的结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T RandProb<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            if (source == null || source.Count() == 0)
            {
                return default(T);
            }
            if (source.Count() == 1)
            {
                return source.ElementAt(0);
            }

            int sum = source.Sum(selector);
            int curNum = 0;
            int randNum = random.Next(0, sum);
            foreach (T obj in source)
            {
                curNum += selector(obj);
                if (curNum >= randNum)
                {
                    return obj;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 按照均等概率从一个集合中随机出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T RandProb<T>(this IEnumerable<T> source)
        {
            if (source == null || source.Count() == 0)
            {
                return default(T);
            }
            if (source.Count() == 1)
            {
                return source.ElementAt(0);
            }

            int sum = source.Count();
            int randNum = random.Next(0, sum);
            return source.ElementAt(randNum);
        }

        /// <summary>
        /// 生成区间[start, end)上的整数列
        /// </summary>
        /// <param name="start">数列开始，包含在数列中</param>
        /// <param name="end">数列结束，不包含在数列中</param>
        /// <returns></returns>
        public static List<int> Range(int start, int end)
        {
            List<int> results = new List<int>();
            for(var i = start; i < end; i++)
            {
                results.Add(i);
            }

            return results;
        }

        public static void Swap<T>(this List<T> source, int key1, int key2)
        {
            T obj = source[key1];
            source[key1] = source[key2];
            source[key2] = obj;
        }

        /// <summary>
        /// 对数组任意洗牌
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> Suffle<T>(this IEnumerable<T> source)
        {
            List<T> results = new List<T>();
            var count = source.Count();
            List<int> indexs = Range(0, count);
            for (var i = 0; i < count; i++)
            {
                var rand = RandNext(0, count);
                indexs.Swap(i, rand);
            }

            foreach(var key in indexs)
            {
                results.Add(source.ElementAt(key));
            }

            return results;
        }

        public static IEnumerable<T> Appends<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            List<T> results = new List<T>();
            foreach(var v in source)
            {
                results.Add(v);
            }
            foreach(var v in target)
            {
                results.Add(v);
            }

            return results;
        }

        /// <summary>
        /// 从一个结构体数组中按以下规则随机取得一个数
        /// 结构体中某个字段可以是这样的一系列数，比如a[0]=100,a[1]=200,a[2]=300,a[3]=400
        /// 那么先计算总和为1000，然后从0到1000中随机取得一个数，看位于区间[0,100),[100,300),[300,600),[600,1000)
        /// 中的某一个，则返回对应的结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> RandProbMuti<T>(this IEnumerable<T> source, int count, Func<T, int> selector)
        {
            if (source == null || source.Count() == 0)
            {
                return new List<T>();
            }
            if (source.Count() == 1 || count >= source.Count())
            {
                return source;
            }

            List<T> result = new List<T>();

            for(int i = 0; i < count; i++)
            {
                int sum = source.Where(x => result.IndexOf(x) == -1).Sum(selector);
                int curNum = 0;
                int randNum = random.Next(0, sum);
                foreach (T obj in source)
                {
                    if (result.IndexOf(obj) != -1)
                    {
                        continue;
                    }

                    curNum += selector(obj);
                    if (curNum >= randNum)
                    {
                        result.Add(obj);
                        break;
                    }
                }
            }

            return result;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var s in source)
            {
                action(s);
            }

            return source;
        }

        /// <summary>
        /// 移除某个集合中符合某个条件的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        public static void RemoveAll<T,U>(this Dictionary<T, U> source, Func<U, bool> selector)
        {
            List<T> keyList = new List<T>();
            foreach(var kv in source)
            {
                if(selector(kv.Value))
                {
                    keyList.Add(kv.Key);
                }
            }
            foreach(var key in keyList)
            {
                source.Remove(key);
            }
        }

        public static void RemoveAll<T,U>(this ConcurrentDictionary<T, U> source, Func<U, bool> selector)
        {
            List<T> keyList = new List<T>();
            foreach (var kv in source)
            {
                if (selector(kv.Value))
                {
                    keyList.Add(kv.Key);
                }
            }
            foreach (var key in keyList)
            {
                U val;
                source.TryRemove(key, out val);
            }
        }

        /// <summary>
        /// 对一个集合按照一定规则
        /// </summary>
        /// <typeparam name="T">集合Key类型</typeparam>
        /// <typeparam name="U">集合Value类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">合并key的选择器</param>
        /// <returns>分组后的集合</returns>
        public static Dictionary<T, List<U>> Classify<T,U>(this IEnumerable<U> source, Func<U, T> selector)
        {
            Dictionary<T, List<U>> obj = new Dictionary<T, List<U>>();
            foreach (var v in source)
            {
                var key = selector(v);
                if (!obj.ContainsKey(key))
                {
                    obj[key] = new List<U>();
                }
                obj[key].Add(v);
            }
            return obj;
        }

        public static DateTime GetDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 判断两条是否处于同一天
        /// </summary>
        /// <param name="dtTime1"></param>
        /// <param name="dtTime2"></param>
        /// <returns></returns>
        public static bool IsSameDay(this DateTime dtTime1, DateTime dtTime2)
        {
            return (dtTime1.Year == dtTime2.Year) && (dtTime1.Month == dtTime2.Month) && (dtTime1.Day == dtTime2.Day);
        }

        /// <summary>
        /// 取得两个日期之间相差多少天
        /// </summary>
        /// <param name="dtTime1"></param>
        /// <param name="dtTime2"></param>
        /// <returns></returns>
        public static int GetTotalDays(this DateTime dtTime1, DateTime dtTime2)
        {
            var dtTime1Zero = new DateTime(dtTime1.Year, dtTime1.Month, dtTime1.Day);
            var dtTime2Zero = new DateTime(dtTime2.Year, dtTime2.Month, dtTime2.Day);

            if (dtTime1Zero >= dtTime2Zero)
            {
                return (int)(dtTime1Zero - dtTime2Zero).TotalDays;
            }
            else
            {
                return (int)(dtTime2Zero - dtTime1Zero).TotalDays;
            }
        }

        /// <summary>
        /// 取得一周的第一天，以星期天为第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDay(DateTime dateTime)
        {
            int week = (int)dateTime.DayOfWeek;
            dateTime = dateTime.AddDays(0 - week);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 取得两个日期之间相差多少个星期
        /// </summary>
        /// <param name="dtTime1"></param>
        /// <param name="dtTime2"></param>
        /// <returns></returns>
        public static int GetTotalWeeks(DateTime dtTime1, DateTime dtTime2)
        {
            DateTime dtTime1Zero;
            DateTime dtTime2Zero;
            if (dtTime1 < dtTime2)
            {
                dtTime1Zero = GetWeekFirstDay(dtTime1);
                dtTime2Zero = GetWeekFirstDay(dtTime2);
            }
            else
            {
                dtTime2Zero = GetWeekFirstDay(dtTime1);
                dtTime1Zero = GetWeekFirstDay(dtTime2);
            }

            return (int)Math.Round(((dtTime2 - dtTime1).TotalDays / 7));
        }

        /// <summary>
        /// 判断两个日期是否是同一个星期，以星期一为每一周的开始
        /// </summary>
        /// <param name="dtTime1"></param>
        /// <param name="dtTime2"></param>
        /// <returns></returns>
        public static bool IsSameWeek(this DateTime dtTime1, DateTime dtTime2)
        {
            if (dtTime1 == DateTime.MinValue || dtTime2 == DateTime.MinValue
                || dtTime1 == DateTime.MaxValue || dtTime2 == DateTime.MaxValue)
            {
                return false;
            }

            Func<DateTime, int> getDayAction = (date) =>
            {
                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                var weekDay = cal.GetDayOfWeek(date);
                if (weekDay == DayOfWeek.Sunday)
                {
                    return 0;
                }
                return (int)weekDay;
            };

            var d1 = dtTime1.Date.AddDays(-1 * getDayAction(dtTime1));
            var d2 = dtTime2.Date.AddDays(-1 * getDayAction(dtTime2));

            return d1 == d2;
        }

        public static int GetWeekDistance(this DateTime dtTime1, DateTime dtTime2)
        {
            if (IsSameWeek(dtTime1, dtTime2))
            {
                return 0;
            }

            var minDate = dtTime1 > dtTime2 ? dtTime2 : dtTime1;
            var maxDate = dtTime1 > dtTime2 ? dtTime1 : dtTime2;

            var weeks = 0;
            while(minDate < maxDate)
            {
                weeks++;
                minDate = minDate.AddDays(7);
            }

            return weeks;
        }


        /// <summary>
        /// 检测数组中是否存在重复的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>存在重复元素返回true，不存在重复元素返回false</returns>
        public static bool CheckRepeated<T>(IEnumerable<T> source)
        {
            var count = source.Count();
            for(var i = 0; i < count; i++)
            {
                var objTest = source.ElementAt(i);
                for(var j = i+1; j < count; j++)
                {
                    if (objTest.Equals(source.ElementAt(j)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int GetTimeStamp(DateTime date)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(date);
            var timeStamp = (Int32)(utcTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return timeStamp;
        }

        public static DateTime GetDateTime(long timeStamp)
        {
            // DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return startTime.AddSeconds(timeStamp);
        }

        /// <summary>
        /// 将IP地址转为长整形
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static long IpToNumber(string ip)
        {
            string[] arr = ip.Split('.');
            return 256 * 256 * 256 * long.Parse(arr[0]) + 256 * 256 * long.Parse(arr[1]) + 256 * long.Parse(arr[2]) + long.Parse(arr[3]);
        }

        /// <summary>
        /// C#判断IP地址是否为私有/内网ip地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="filter">测试IP地址类型，"ABCO"类地址的组合</param>
        /// <returns></returns>
        public static bool IsPrivateIp(string ip, string filter = "ABCO")
        {
            long ABegin = IpToNumber("10.0.0.0"), AEnd = IpToNumber("10.255.255.255");          //A类私有IP地址
            long BBegin = IpToNumber("172.16.0.0"), BEnd = IpToNumber("172.31.255.255");        //B类私有IP地址
            long CBegin = IpToNumber("192.168.0.0"), CEnd = IpToNumber("192.168.255.255");      //C类私有IP地址
            long OBegin = IpToNumber("127.0.0.1"), OEnd = IpToNumber("127.0.0.1");              //O类私有IP地址
            long IpNum = IpToNumber(ip);

            bool result = false;
            if (filter.Contains("A"))
            {
                result = result || (ABegin <= IpNum && IpNum <= AEnd);
            }
            if (filter.Contains("B"))
            {
                result = result || (BBegin <= IpNum && IpNum <= BEnd);
            }
            if (filter.Contains("C"))
            {
                result = result || (CBegin <= IpNum && IpNum <= CEnd);
            }
            if (filter.Contains("O"))
            {
                result = result || (OBegin <= IpNum && IpNum <= OEnd);
            }

            return result;
        }


        public static List<IPAddress> GetAllDnsHost()
        {
            List<IPAddress> ipAddress = new List<IPAddress>();
            ipAddress.Add(IPAddress.Parse("0.0.0.0"));
            ipAddress.Add(IPAddress.Parse("::0"));
            return ipAddress;
        }

        public static List<IPAddress> GetIpAddress(string host)
        {
            if (host.ToLower() == "any")
            {
                return GetAllDnsHost();
            }
            else
            {
                var ip = IPAddress.Parse(host);
                return new List<IPAddress>() { ip };
            }
        }

        private static bool TestPortIsUsed(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }

        public static int GetUnusedPort()
        {
            int port = 0;
            bool isPortValid = false;
            do
            {
                port = RandNext(2000, 40000);
                isPortValid = !TestPortIsUsed(port);
            }
            while (!isPortValid);

            return port;
        }

        public static int GetConfigPort(string portStr)
        {
            if (portStr.ToLower() == "auto")
            {
                return GetUnusedPort();
            }
            else
            {
                return int.Parse(portStr);
            }
        }

        public static bool IsLocal(IPAddress address)
        {
            if (address == null)
                return false;

            if (address.Equals(IPAddress.Any))
                return true;

            if (address.Equals(IPAddress.Loopback))
                return true;

            if (Socket.OSSupportsIPv6)
            {
                if (address.Equals(IPAddress.IPv6Any))
                    return true;

                if (address.Equals(IPAddress.IPv6Loopback))
                    return true;
            }

            var host = Dns.GetHostName();
            var addrs = Dns.GetHostAddresses(host);
            foreach (var addr in addrs)
            {
                if (address.Equals(addr))
                    return true;
            }

            return false;
        }

        public static async Task<string> SendRequestGet(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url);
                return result;
            }
        }

        public static async Task<string> SendRequestGetTimeOut(string url, int timeout = 2000)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url).TimeoutAfter(new TimeSpan(0, 0, 0, 0, timeout));
                return result;
            }
        }


        public static async Task<string> SendRequestPost(string url, string postData, string contentType = null)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(postData);
                if (contentType != null)
                {
                    content.Headers.ContentType.MediaType = contentType;
                }
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }

        public static async Task<string> SendRequestPost(string url, byte[] postData, string contentType = null)
        {
            using (var client = new HttpClient())
            {
                var content = new ByteArrayContent(postData);
                if (contentType != null)
                {
                    content.Headers.ContentType.MediaType = contentType;
                }
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }



        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }

        public static string MD5(byte[] buffer)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(buffer);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString().ToLower();
        }

        public static string MD5(string src)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(src);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString().ToLower();
        }

        public static bool TrueForAny<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            foreach(var obj in source)
            {
                if (match(obj))
                {
                    return true;
                }
            }

            return false;
        }

        public static string SafeToString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        public static string GetFileExt(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf(".") + 1);
        }
    }
}
