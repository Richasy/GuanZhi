using Newtonsoft.Json;
using Project_GuanZhi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Project_GuanZhi.Tools
{
    public class IOTools
    {
        public static async Task<StorageFolder> GetLocalDataFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var dataFolder = await localFolder.CreateFolderAsync("UserData", CreationCollisionOption.OpenIfExists);
            return dataFolder;
        }

        /// <summary>
        /// 获取本地最近打开文章列表
        /// </summary>
        /// <returns></returns>
        public async static Task<List<AppArticleModel>> GetLocalRecentArticleList()
        {
            var dataFolder = await GetLocalDataFolder();
            var recentArticleFile = await dataFolder.CreateFileAsync("RecentArticle.json", CreationCollisionOption.OpenIfExists);
            string fileJson = await FileIO.ReadTextAsync(recentArticleFile);
            if (string.IsNullOrEmpty(fileJson))
            {
                fileJson = "[]";
            }
            return JsonConvert.DeserializeObject<List<AppArticleModel>>(fileJson);
        }

        /// <summary>
        /// 重写本地最近打开文章列表
        /// </summary>
        /// <param name="recentArticleCollection">更改后的文章列表</param>
        /// <returns></returns>
        public async static Task<bool> RewriteLocalRecentArticleList(ICollection<AppArticleModel> recentArticleCollection)
        {
            var dataFolder = await GetLocalDataFolder();
            string writeJson = JsonConvert.SerializeObject(recentArticleCollection);
            var recentArticleFile = await dataFolder.CreateFileAsync("RecentArticle.json", CreationCollisionOption.OpenIfExists);
            try
            {
                await FileIO.WriteTextAsync(recentArticleFile, writeJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取本地收藏文章列表
        /// </summary>
        /// <returns></returns>
        public async static Task<List<AppArticleModel>> GetLocalFavouriteArticleList()
        {
            var dataFolder = await GetLocalDataFolder();
            var favouriteArticleFile = await dataFolder.CreateFileAsync("FavouriteArticle.json", CreationCollisionOption.OpenIfExists);
            string fileJson = await FileIO.ReadTextAsync(favouriteArticleFile);
            if (string.IsNullOrEmpty(fileJson))
            {
                fileJson = "[]";
            }
            return JsonConvert.DeserializeObject<List<AppArticleModel>>(fileJson);
        }

        /// <summary>
        /// 重写本地收藏文章列表
        /// </summary>
        /// <param name="favouriteArticleCollection">更改后的文章列表</param>
        /// <returns></returns>
        public async static Task<bool> RewriteLocalFavouriteArticleList(ICollection<AppArticleModel> favouriteArticleCollection)
        {
            var dataFolder = await GetLocalDataFolder();
            string writeJson = JsonConvert.SerializeObject(favouriteArticleCollection);
            var favouriteArticleFile = await dataFolder.CreateFileAsync("FavouriteArticle.json", CreationCollisionOption.OpenIfExists);
            try
            {
                await FileIO.WriteTextAsync(favouriteArticleFile, writeJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 追加单个收藏文章
        /// </summary>
        /// <param name="favouriteArticle">收藏文章</param>
        /// <returns></returns>
        public async static Task<bool?> AddLocalFavouriteArticleItem(AppArticleModel favouriteArticle)
        {
            var dataFolder = await GetLocalDataFolder();
            var favouriteArticleFile = await dataFolder.CreateFileAsync("FavouriteArticle.json", CreationCollisionOption.OpenIfExists);
            string sourceJson = await FileIO.ReadTextAsync(favouriteArticleFile);
            if (string.IsNullOrEmpty(sourceJson))
            {
                sourceJson = "[]";
            }
            var sourceList = JsonConvert.DeserializeObject<List<AppArticleModel>>(sourceJson);
            if (sourceList.Any(article => article.Title == favouriteArticle.Title))
            {
                return null;
            }
            sourceList.Add(favouriteArticle);
            string afterJson = JsonConvert.SerializeObject(sourceList);
            try
            {
                await FileIO.WriteTextAsync(favouriteArticleFile, afterJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
