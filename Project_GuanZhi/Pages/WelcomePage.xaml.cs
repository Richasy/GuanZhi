using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            rootFrame.Navigate(typeof(MainPage));
        }
    }
}
