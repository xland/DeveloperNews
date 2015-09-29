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
    public partial class Default : System.Web.UI.Page
    {
        OrmLiteConnectionFactory dbFactory;
        protected void Page_Load(object sender, EventArgs e)
        {
            dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.AppSettings["dbConnStr"], MySqlDialect.Provider);            
            var action = Request["Action"];
            var id = Request["Id"];
            List<allen_news> result = null;            
            if (action == "PullDown")
            {
                var db = dbFactory.Open();
                result = db.SelectFmt<allen_news>("select * from allen_news where news_id > {0} order by news_id desc limit 0,30",id);
                db.Dispose();
            }
            else if(action == "PullUp")
            {
                var db = dbFactory.Open();
                result = db.SelectFmt<allen_news>("select * from allen_news where news_id < {0} order by news_id desc limit 0,30", id);
                db.Dispose();
            }
            else if(action == "CheckVersion")
            {
                Response.Write("1.0.0");
                Response.End();
                return;
            }
            else
            {
                var db = dbFactory.Open();
                result = db.Select<allen_news>("select * from allen_news order by news_id desc limit 0,30");
                db.Dispose();
            }
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }
    }
}