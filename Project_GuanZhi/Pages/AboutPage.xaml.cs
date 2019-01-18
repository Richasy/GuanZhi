using Newtonsoft.Json;
using Project_GuanZhi.Controls;
using Project_GuanZhi.Models;
using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Project_GuanZhi.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private bool IsInit = false;
        public AboutPage()
        {
            this.InitializeComponent();
            SettingPageInit();
        }

        private void SettingPageInit()
        {
            string userName = AppTools.GetLocalSetting(Models.AppSettings.UserName, "");
            UserNameRun.Text = userName;
            bool isLight = AppTools.GetLocalSetting(Models.AppSettings.Theme, "Light") == "Light";
            if (isLight)
                LightRadioButton.IsChecked = true;
            else
                DarkRadioButton.IsChecked = true;
            IsInit = true;
            int startTime = Convert.ToInt32(AppTools.GetLocalSetting(AppSettings.FirstRunTime, "0"));
            var startDate = AppTools.GetDateTime(startTime);
            var span = DateTime.Now - startDate;
            DurationDayRun.Text = span.Days.ToString();
            ReadCountRun.Text = MainPage.Current.ReadArticleCollection.Count.ToString();
            double wordCount = 0;
            foreach (var article in MainPage.Current.ReadArticleCollection)
            {
                wordCount += article.WordCount;
            }
            if (wordCount > 10000)
            {
                WordCountRun.Text = Math.Round(wordCount / 10000, 2) + "万";
            }
            else
            {
                WordCountRun.Text = wordCount.ToString();
            }
        }

        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsInit)
            {
                return;
            }
            string name = ((RadioButton)sender).Name;
            if (name.IndexOf("Light", StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                MainPage.Current.MainPageTheme = ElementTheme.Light;
                MainPage.Current.RequestedTheme = ElementTheme.Light;
                AppTools.WriteLocalSetting(Models.AppSettings.Theme, "Light");
            }
            else
            {
                MainPage.Current.MainPageTheme = ElementTheme.Dark;
                MainPage.Current.RequestedTheme = ElementTheme.Dark;
                AppTools.WriteLocalSetting(Models.AppSettings.Theme, "Dark");
            }
            foreach (var sideMenu in MainPage.Current.SideMenuCollection)
            {
                sideMenu.UpdateLayout();
            } 
            new PopToast("主题已切换，请重启软件以获得更好的体验").ShowPopup();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            if (windowWidth < 1000)
            {
                Grid.SetColumn(OptionContainer, 0);
                Grid.SetRow(OptionContainer, 1);
            }
            else
            {
                Grid.SetColumn(OptionContainer, 1);
                Grid.SetRow(OptionContainer, 0);
            }
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await IOTools.OpenLocalFile(".guanzhi");
            if (file != null)
            {
                string text = await FileIO.ReadTextAsync(file);
                var model = JsonConvert.DeserializeObject<ExportModel>(text);
                var tipDialog = new ConfirmDialog("导入提醒", $"您将导入用户名为'{model.UserName}'用户的相关配置及历史记录。这会覆盖您目前的配置及记录，是否确认？");
                var result = await tipDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    bool importResult=await ExportModel.ImportModel(model);
                    if (importResult)
                    {
                        var closeDialog = new ConfirmDialog("请重启软件", "配置及历史记录已成功导入，现在请关闭软件，重新启动应用","关闭","关闭","还是关闭");
                        await closeDialog.ShowAsync();
                        App.Current.Exit();
                    }
                }
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var exportData = await ExportModel.GenerateExportModel();
            string writeJson = JsonConvert.SerializeObject(exportData);
            var file = await IOTools.SaveLocalFile(".guanzhi", "观止UWP配置文件"+DateTime.Now.ToString("yyyyMMdd"));
            if (file != null)
            {
                await FileIO.WriteTextAsync(file,writeJson);
                new PopToast("数据已导出").ShowPopup();
            }
        }
    }
}
