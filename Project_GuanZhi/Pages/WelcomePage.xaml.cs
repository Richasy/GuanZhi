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
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isEmpty = string.IsNullOrEmpty(UserNameTextBox.Text.Trim());
            if (isEmpty)
            {
                ReadyButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReadyButton.Visibility = Visibility.Visible;
            }
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UserNameTextBox.Text;
            AppTools.WriteLocalSetting(Models.AppSettings.UserName, name);
            AppTools.WriteLocalSetting(Models.AppSettings.IsFirstRun, "False");
            var rootFrame = Window.Current.Content as Frame;
            AppTools.WriteLocalSetting(Models.AppSettings.FirstRunTime, AppTools.GetTimeUnix(DateTime.Now.ToUniversalTime()).ToString());
            rootFrame.Navigate(typeof(MainPage));
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
                    bool importResult = await ExportModel.ImportModel(model);
                    if (importResult)
                    {
                        var closeDialog = new ConfirmDialog("请重启软件", "配置及历史记录已成功导入，现在请关闭软件，重新启动应用", "关闭", "关闭", "还是关闭");
                        await closeDialog.ShowAsync();
                        App.Current.Exit();
                    }
                }
            }
        }
    }
}
