using Entity;
using Newtonsoft.Json;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebService
{
    public partial class ViewService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pageSize = 8;
            var Index = Request["Index"];
            if (string.IsNullOrWhiteSpace(Index))
            {
                Index = "0";
            }
            var start = Convert.ToInt32(Index);
            var dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.AppSettings["dbConnStr"], MySqlDialect.Provider);
            var db = dbFactory.Open();
            List<allen_news> obj;
            if(start == 0)
            {
                obj = db.Select<allen_news>("select * from allen_news order by news_id desc limit 0,"+(pageSize*2).ToString());
            }
            else
            {
                obj = db.Select<allen_news>("select * from allen_news where news_id < "+start.ToString()+" order by news_id desc limit 0," + pageSize.ToString());
            }
            db.Close();
            db.Dispose();
            if (obj.Count < 1)
            {
                Response.Write("Sorry...我们不打算把不算“新”闻的内容提供给您，您收藏的内容可以在“我的收藏”中找到。");
                Response.End();
                return;
            }
            Response.Write(JsonConvert.SerializeObject(obj));
            Response.End();
        }
    }
}