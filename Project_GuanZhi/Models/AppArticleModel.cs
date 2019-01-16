using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_GuanZhi.Models
{
    public class AppArticleModel
    {
        public string Date { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Digest { get; set; }
        public int WordCount { get; set; }
    }
}
