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
        public AppArticleModel()
        {

        }
        public AppArticleModel(WebArticleData webData)
        {
            Date = webData.Date.Curr;
            Author = webData.Author;
            Title = webData.Title;
            Digest = webData.Digest;
            WordCount = webData.Wc;
        }

        public override bool Equals(object obj)
        {
            var model = obj as AppArticleModel;
            return model != null &&
                   Date == model.Date &&
                   Title == model.Title;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502420234;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Date);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            return hashCode;
        }
    }
}
