using Newtonsoft.Json;
using Project_GuanZhi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Project_GuanZhi.Tools
{
    public class AppTools
    {
        public const string TODAY_URL = "https://interface.meiriyiwen.com/article/today?dev=1";
        public const string SOMEDAY_URL = "https://interface.meiriyiwen.com/article/day?dev=1&date=";
        public const string RANDOM_URL = "https://interface.meiriyiwen.com/article/random?dev=1";
        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="url">获取地址</param>
        /// <returns>指定类型的对象</returns>
        public static async Task<T> GetAsyncJson<T>(string url)
        {
            try
            {
                var http = new HttpClient();
                http.Timeout = new TimeSpan(0, 1, 0);
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(result);
                http.Dispose();
                return data;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return default(T);
            }
        }
        /// <summary>
        /// 写入本地设置
        /// </summary>
        /// <param name="key">设置名</param>
        /// <param name="value">设置值</param>
        public static void WriteLocalSetting(AppSettings key, string value)
        {
            var localSetting = ApplicationData.Current.LocalSettings;
            var localcontainer = localSetting.CreateContainer("GuanZhi", ApplicationDataCreateDisposition.Always);
            localcontainer.Values[key.ToString()] = value;
        }
        /// <summary>
        /// 读取本地设置
        /// </summary>
        /// <param name="key">设置名</param>
        /// <returns></returns>
        public static string GetLocalSetting(AppSettings key, string defaultValue)
        {
            var localSetting = ApplicationData.Current.LocalSettings;
            var localcontainer = localSetting.CreateContainer("GuanZhi", ApplicationDataCreateDisposition.Always);
            bool isKeyExist = localcontainer.Values.ContainsKey(key.ToString());
            if (isKeyExist)
            {
                return localcontainer.Values[key.ToString()].ToString();
            }
            else
            {
                WriteLocalSetting(key, defaultValue);
                return defaultValue;
            }
        }
        /// <summary>
        /// 初始化标题栏颜色
        /// </summary>
        public static void SetTitleBarColorInit()
        {
            var view = ApplicationView.GetForCurrentView();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var Theme = GetLocalSetting(AppSettings.Theme, "Light");
            if (Theme.ToLower() == "dark")
            {
                // active
                view.TitleBar.BackgroundColor = Colors.Transparent;
                view.TitleBar.ForegroundColor = Colors.White;

                // inactive
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveForegroundColor = Colors.Gray;
                // button
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonForegroundColor = Colors.White;

                view.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 33, 42, 67);
                view.TitleBar.ButtonHoverForegroundColor = Colors.White;

                view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 255, 86, 86);
                view.TitleBar.ButtonPressedForegroundColor = Colors.White;

                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
            else
            {
                // active
                view.TitleBar.BackgroundColor = Colors.Transparent;
                view.TitleBar.ForegroundColor = Colors.Black;

                // inactive
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveForegroundColor = Colors.Gray;
                // button
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonForegroundColor = Colors.DarkGray;

                view.TitleBar.ButtonHoverBackgroundColor = Colors.LightGray;
                view.TitleBar.ButtonHoverForegroundColor = Colors.DarkGray;

                view.TitleBar.ButtonPressedBackgroundColor = Colors.DarkGray;
                view.TitleBar.ButtonPressedForegroundColor = Colors.White;

                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
        }
        /// <summary>
        /// 获取当前指定的父控件
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="obj">控件</param>
        /// <param name="name">父控件名</param>
        /// <returns></returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        /// <summary>
        /// 获取当前控件的指定子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="obj">父控件</param>
        /// <param name="name">子控件名</param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                }
                if (grandChild != null)
                {
                    return grandChild;
                }
            }
            return null;
        }

        /// <summary>
        /// 标准化字符串，去掉空格，全部小写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string NormalString(string str)
        {
            str = str.ToLower();
            var reg = new Regex(@"\s", RegexOptions.IgnoreCase);
            str = reg.Replace(str, "");
            return str;
        }

        /// <summary>
        /// 获取预先定义的线性画笔资源
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static SolidColorBrush GetThemeSolidColorBrush(string key)
        {
            return (SolidColorBrush)App.Current.Resources[key];
        }

        /// <summary>
        /// 获取当前Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static int GetTimeUnix(DateTime date)
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int seconds = Convert.ToInt32(ts.TotalSeconds);
            return seconds;
        }
        /// <summary>
        /// 从Unix时间戳获取DateTime时间
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns></returns>
        public static DateTime GetDateTime(int seconds)
        {
            var basicTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            basicTime=basicTime.AddSeconds(seconds);
            return basicTime;
        }

        /// <summary>
        /// 从日期获取DateTime时间
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string time)
        {
            int year = Convert.ToInt32(time.Substring(0, 4));
            int month = Convert.ToInt32(time.Substring(4, 2));
            int day = Convert.ToInt32(time.Substring(6));
            var date = new DateTime(year, month, day,0,0,0,0);
            return date;
        }
    }
}
