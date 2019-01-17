using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_GuanZhi.Models
{
    public class ExportModel
    {
        public string UserName { get; set; }
        public int FirstRunTime { get; set; }
        public string ReadFontFamily { get; set; }
        public int ReadFontSize { get; set; }
        public double ReadLineHeight { get; set; }
        public double ReadParagraphHeight { get; set; }
        public string AppTheme { get; set; }
        public List<AppArticleModel> ReadArticle { get; set; }
        public List<AppArticleModel> FavouriteArticle { get; set; }
        public List<AppArticleModel> RecentArticle { get; set; }

        public static async Task<ExportModel> GenerateExportModel()
        {
            var model = new ExportModel();
            model.UserName = AppTools.GetLocalSetting(AppSettings.UserName, "");
            model.FirstRunTime = Convert.ToInt32(AppTools.GetLocalSetting(AppSettings.FirstRunTime, "0"));
            model.ReadFontFamily = AppTools.GetLocalSetting(AppSettings.ReadFontFamily, "");
            model.ReadFontSize = Convert.ToInt32(AppTools.GetLocalSetting(AppSettings.ReadFontSize, "16"));
            model.ReadLineHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadLineHeight, "1.5"));
            model.ReadParagraphHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadParagraphHeight, "0.5"));
            model.AppTheme = AppTools.GetLocalSetting(AppSettings.Theme,"Light");
            model.ReadArticle = await IOTools.GetLocalReadArticleList();
            model.FavouriteArticle = await IOTools.GetLocalFavouriteArticleList();
            model.RecentArticle = await IOTools.GetLocalRecentArticleList();
            return model;
        }

        public static async Task<bool> ImportModel(ExportModel model)
        {
            AppTools.WriteLocalSetting(AppSettings.FirstRunTime, model.FirstRunTime.ToString());
            AppTools.WriteLocalSetting(AppSettings.ReadFontFamily, model.ReadFontFamily);
            AppTools.WriteLocalSetting(AppSettings.ReadFontSize, model.ReadFontSize.ToString());
            AppTools.WriteLocalSetting(AppSettings.ReadLineHeight, model.ReadLineHeight.ToString());
            AppTools.WriteLocalSetting(AppSettings.ReadParagraphHeight, model.ReadParagraphHeight.ToString());
            AppTools.WriteLocalSetting(AppSettings.Theme, model.AppTheme);
            AppTools.WriteLocalSetting(AppSettings.UserName, model.UserName);
            try
            {
                await IOTools.RewriteLocalFavouriteArticleList(model.FavouriteArticle);
                await IOTools.RewriteLocalReadArticleList(model.ReadArticle);
                await IOTools.RewriteLocalRecentArticleList(model.RecentArticle);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
