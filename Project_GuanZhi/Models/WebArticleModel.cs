using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_GuanZhi.Models
{
    public class WebArticleModel
    {
        public WebArticleData Data { get; set; }
    }
    public class WebArticleData
    {
        public WebArticleDate Date { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Digest { get; set; }
        public string Content { get; set; }
        public int Wc { get; set; }
    }
    public class WebArticleDate
    {
        public string Curr { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
    }
}
