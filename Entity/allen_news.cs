using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class allen_news
    {
        public long news_id
        {
            get;
            set;
        }
        public string news_title
        {
            get;
            set;
        }
        public string news_summary
        {
            get;
            set;
        }
        public int from_site_flag
        {
            get;
            set;
        }
        public string author
        {
            get;
            set;
        }
        public DateTime add_time
        {
            get;
            set;
        }
        public string news_url
        {
            get;
            set;
        }
    }
}
