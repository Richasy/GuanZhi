using Newtonsoft.Json;
using Project_GuanZhi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

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
        /// 获取本地已读文章列表
        /// </summary>
        /// <returns></returns>
        public async static Task<List<AppArticleModel>> GetLocalReadArticleList()
        {
            var dataFolder = await GetLocalDataFolder();
            var readArticleFile = await dataFolder.CreateFileAsync("ReadArticle.json", CreationCollisionOption.OpenIfExists);
            string fileJson = await FileIO.ReadTextAsync(readArticleFile);
            if (string.IsNullOrEmpty(fileJson))
            {
                fileJson = "[]";
            }
            return JsonConvert.DeserializeObject<List<AppArticleModel>>(fileJson);
        }

        /// <summary>
        /// 重写本地已读文章列表
        /// </summary>
        /// <param name="readArticleCollection">更改后的文章列表</param>
        /// <returns></returns>
        public async static Task<bool> RewriteLocalReadArticleList(ICollection<AppArticleModel> readArticleCollection)
        {
            var dataFolder = await GetLocalDataFolder();
            string writeJson = JsonConvert.SerializeObject(readArticleCollection);
            var readArticleFile = await dataFolder.CreateFileAsync("ReadArticle.json", CreationCollisionOption.OpenIfExists);
            try
            {
                await FileIO.WriteTextAsync(readArticleFile, writeJson);
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

        /// <summary>
        /// 打开本地文件
        /// </summary>
        /// <param name="types">后缀名列表(如.jpg,.mp3等)</param>
        /// <returns>单个文件</returns>
        public async static Task<StorageFile> OpenLocalFile(params string[] types)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            Regex typeReg = new Regex(@"^\.[a-zA-Z0-9]+$");
            foreach (var type in types)
            {
                if (type == "*" || typeReg.IsMatch(type))
                    picker.FileTypeFilter.Add(type);
                else
                    throw new InvalidCastException("文件后缀名不正确");
            }
            var file = await picker.PickSingleFileAsync();
            if (file != null)
                return file;
            else
                return null;
        }

        /// <summary>
        /// 存储指定类型的文件
        /// </summary>
        /// <param name="type">文件后缀名，带'.'</param>
        /// <param name="suggestName">建议文件名</param>
        /// <param name="file">写入文件</param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveLocalFile(string type, string suggestName)
        {
            var picker = new FileSavePicker();
            picker.DefaultFileExtension = type;
            picker.FileTypeChoices.Add(type + " File", new List<string>() { type });
            picker.SuggestedFileName = suggestName;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile saveFile = await picker.PickSaveFileAsync();
            return saveFile;
        }
    }
}
