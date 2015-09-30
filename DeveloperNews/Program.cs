using CsQuery;
using RestSharp;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using Entity;
using System.Net;
using System.IO;

namespace DeveloperNews
{
    class Program
    {
        static OrmLiteConnectionFactory dbFactory;
        static void Main(string[] args)
        {
            dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.AppSettings["dbConnStr"], MySqlDialect.Provider);

            var Tcto51 = new Thread(new ThreadStart(cto51));
            Tcto51.Start();

            var Tkr36 = new Thread(new ThreadStart(kr36));
            Tkr36.Start();

            var Toschina = new Thread(new ThreadStart(oschina));
            Toschina.Start();

            var Tinfoq = new Thread(new ThreadStart(infoq));
            Tinfoq.Start();

            var Tcnblogs = new Thread(new ThreadStart(cnblogs));
            Tcnblogs.Start();
        }
        static int GetWaitTime()
        {
            Random ran = new Random();
            int n = ran.Next(60000, 600000);//1分钟到10分钟之间的随机数
            return n;
        }
        static bool checkTitle(string title)
        {
            var db = dbFactory.Open();
            var checkData = db.ScalarFmt<long>("select count(*) from allen_news where news_title = {0}", title);
            db.Dispose();
            if (checkData > 0)
            {
                return true;
            }
            return false;
        }
        static void cto51()
        {
            CQ doc;
            try
            {
                HttpWebRequest request = WebRequest.Create("http://www.51cto.com/recommnews/list1.htm") as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.Default);
                string html = reader.ReadToEnd();
                stream.Close();
                doc = html;
            }
            catch (Exception ex)
            {
                Thread.Sleep(GetWaitTime());
                cto51();
                return;
            }
            var arr = doc[".list-li"].ToList();
            var dataList = new List<allen_news>();
            var db = dbFactory.Open();
            foreach (var item in arr)
            {
                CQ target = item.InnerHTML;
                var data = new allen_news();
                data.news_title = target[".pic a"].Text().Trim();
                if (checkTitle(data.news_title))
                {
                    continue;
                }
                data.news_summary = target[".cont .info"].Text().Trim();
                data.author = "";
                data.add_time = DateTime.Now;
                data.from_site_flag = 4;
                data.news_url = target[".pic a"].Attr("href");
                dataList.Insert(0, data);
            }
            if (dataList.Count > 0)
            {
                db.InsertAll<allen_news>(dataList);
            }
            db.Dispose();
            Console.WriteLine("增加了{0}条文章4", dataList.Count);
            Thread.Sleep(GetWaitTime());
            cto51();
        }

        static void kr36()
        {
            CQ doc;
            try
            {
                var client = new RestClient("http://36kr.com/");
                var resq = new RestRequest(Method.GET);
                var resp = client.Execute(resq);
                doc = resp.Content;
            }
            catch (Exception ex)
            {
                Thread.Sleep(GetWaitTime());
                kr36();
                return;
            }
            var arr = doc["article .desc"].ToList();
            var dataList = new List<allen_news>();
            var db = dbFactory.Open();
            foreach (var item in arr)
            {
                CQ target = item.InnerHTML;
                var data = new allen_news();
                data.news_title = target[".title"].Text().Trim();
                if (checkTitle(data.news_title))
                {
                    continue;
                }
                data.news_summary = target[".brief"].Text().Trim();
                data.author = target[".author a"].Text().Trim();
                data.add_time = DateTime.Now;
                data.from_site_flag = 3;
                data.news_url = "http://36kr.com" + target[".title"].Attr("href");
                dataList.Insert(0, data);
            }
            if (dataList.Count > 0)
            {
                db.InsertAll<allen_news>(dataList);
            }
            db.Dispose();
            Console.WriteLine("增加了{0}条文章3", dataList.Count);
            Thread.Sleep(GetWaitTime());
            kr36();
        }

        static void oschina()
        {
            CQ doc;
            try
            {
                var client = new RestClient("http://www.oschina.net/news/list");
                var resq = new RestRequest(Method.GET);
                var resp = client.Execute(resq);
                doc = resp.Content;
            }
            catch (Exception ex)
            {
                Thread.Sleep(GetWaitTime());
                oschina();
                return;
            }
            var arr = doc["#RecentNewsList .List li"].ToList();
            var dataList = new List<allen_news>();
            var db = dbFactory.Open();
            foreach (var item in arr)
            {
                CQ target = item.InnerHTML;
                var data = new allen_news();
                data.news_title = target["h2 a"].Text().Trim();
                if (checkTitle(data.news_title))
                {
                    continue;
                }
                data.news_summary = target[".detail"].Text().Trim();
                data.author = target[".date a"].Text().Trim();
                data.add_time = DateTime.Now;
                data.from_site_flag = 2;
                data.news_url = "http://www.oschina.net" + target["h2 a"].Attr("href");
                dataList.Insert(0, data);
            }
            if (dataList.Count > 0)
            {
                db.InsertAll<allen_news>(dataList);
            }
            db.Dispose();
            Console.WriteLine("增加了{0}条文章2", dataList.Count);
            Thread.Sleep(GetWaitTime());
            oschina();
        }
        static void infoq()
        {
            CQ doc;
            try
            {
                var client = new RestClient("http://www.infoq.com/cn/news");
                var resq = new RestRequest(Method.GET);
                var resp = client.Execute(resq);
                doc = resp.Content;
            }
            catch (Exception ex)
            {
                Thread.Sleep(GetWaitTime());
                infoq();
                return;
            }
            var arr = doc[".news_type_block"].ToList();
            var dataList = new List<allen_news>();
            var db = dbFactory.Open();
            foreach (var item in arr)
            {
                CQ target = item.InnerHTML;
                var data = new allen_news();
                data.news_title = target["h2 a"].Text().Trim();
                if (checkTitle(data.news_title))
                {
                    continue;
                }
                data.news_summary = target["p"].Text().Trim();
                data.author = target[".author a"].Text().Trim();
                data.add_time = DateTime.Now;
                data.from_site_flag = 1;
                data.news_url = "http://www.infoq.com"+target["h2 a"].Attr("href");
                dataList.Insert(0, data);
            }
            if (dataList.Count > 0)
            {
                db.InsertAll<allen_news>(dataList);
            }
            db.Dispose();
            Console.WriteLine("增加了{0}条文章1", dataList.Count);
            Thread.Sleep(GetWaitTime());
            infoq();
        }
        static void cnblogs()
        {
            CQ doc;
            try
            {
                var client = new RestClient("http://www.cnblogs.com/news/");
                var resq = new RestRequest(Method.GET);
                var resp = client.Execute(resq);
                doc = resp.Content;
            }
            catch (Exception ex)
            {
                Thread.Sleep(GetWaitTime());
                cnblogs();
                return;
            }
            var arr = doc[".post_item_body"].ToList();
            var dataList = new List<allen_news>();
            var db = dbFactory.Open();
            foreach (var item in arr)
            {
                var str = item.InnerText;
                var strArr = str.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                var data = new allen_news();
                data.news_title = strArr[0];
                if (checkTitle(data.news_title))
                {
                    continue;
                }
                data.news_summary = strArr[1].Trim();
                data.author = strArr[2].Split("发布于".ToCharArray(),StringSplitOptions.RemoveEmptyEntries)[0];
                data.add_time = DateTime.Now;
                data.from_site_flag = 0;
                data.news_url = ((CQ)item.InnerHTML)["h3 a"].Attr("href");
                dataList.Insert(0, data);
            }
            if(dataList.Count >0)
            {
                db.InsertAll<allen_news>(dataList);
            }
            db.Dispose();
            Console.WriteLine("增加了{0}条文章0", dataList.Count);
            Thread.Sleep(GetWaitTime());
            cnblogs();
        }
    }
}
