using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace makepagelinkcaoliu
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Error();
                return;
            }
            var url = args[0];
            var uri = new Uri(url);

            try
            {
                var links = GetHyperLinks(GetPageSource(url));
                foreach (var link in links)
                {
                    if (link.StartsWith("htm_data/"))
                    {
                        Console.WriteLine(string.Format("http://{0}/{1}", uri.Host, link));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        static string GetPageSource(string url)
        {
            var uri = new Uri(url);
            var hwReq = (HttpWebRequest)WebRequest.Create(uri);
            var hwRes = (HttpWebResponse)hwReq.GetResponse();
            hwReq.Method = "Get";
            hwReq.KeepAlive = false;
            var reader = new StreamReader(hwRes.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));

            var str = reader.ReadToEnd();
            reader.Close();
            hwRes.Close();

            return str;
        }

        static IEnumerable<string> GetHyperLinks(string htmlCode)
        {
            var lst = new List<string>();

            Regex reg = new Regex(@"(?is)<a(?:(?!href=).)*href=(['""]?)(?<url>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");
            MatchCollection mc = reg.Matches(htmlCode);
            foreach (Match m in mc)
            {
                lst.Add(m.Groups["url"].Value);
            }

            return lst.Distinct();
        }

        private static void Error()
        {
            Console.Error.WriteLine("makepagelinkcaoliu http://www.cl621.com/thread0806.php?fid=2&search=&page=2");
        }
    }
}
