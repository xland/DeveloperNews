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


namespace DeveloperNews
{
    class Program
    {
        static OrmLiteConnectionFactory dbFactory;
        static void Main(string[] args)
        {
            dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.AppSettings["dbConnStr"], MySqlDialect.Provider);
            cnblogs();
            var Tcnblogs = new Thread(new ThreadStart(cnblogs));
            Tcnblogs.Start();
        }
        static int GetWaitTime()
        {
            Random ran = new Random();
            int n = ran.Next(60000, 600000);//1分钟到10分钟之间的随机数
            return n;
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
                var checkData = db.ScalarFmt<long>("select count(*) from allen_news where news_title = {0}", data.news_title);
                if (checkData > 0)
                {
                    break;
                }
                data.news_summary = strArr[1];
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
            Console.WriteLine("增加了{0}条文章", dataList.Count);
            Thread.Sleep(GetWaitTime());
            cnblogs();
        }
    }
}
