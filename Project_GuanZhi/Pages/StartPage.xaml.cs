using Project_GuanZhi.Models;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Project_GuanZhi.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StartPage : Page
    {
        private WebArticleData WebArticle = null;
        public StartPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter!=null && e.Parameter is SideMenuType)
            {
                var navigateType = (SideMenuType)e.Parameter;
                if (navigateType == SideMenuType.Today)
                {
                    TodayArticleInit();
                }
                else if (navigateType == SideMenuType.Random)
                {
                    RandomArticleInit();
                }
            }
        }

        private async void TodayArticleInit()
        {
            RandomButton.Visibility = Visibility.Collapsed;
            WaittingRing.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            var webArticle = await AppTools.GetAsyncJson<WebArticleModel>(AppTools.TODAY_URL);
            UserInterfaceInit(webArticle);
        }

        private async void RandomArticleInit()
        {
            WaittingRing.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            var webArticle = await AppTools.GetAsyncJson<WebArticleModel>(AppTools.RANDOM_URL);
            UserInterfaceInit(webArticle);
        }

        private void UserInterfaceInit(WebArticleModel webArticle)
        {
            MainContent.Visibility = Visibility.Visible;
            WaittingRing.Visibility = Visibility.Collapsed;
            if (webArticle != null)
            {
                WebArticle = webArticle.Data;
                TitleTextBlock.Text = webArticle.Data.Title;
                DigestTextBlock.Text = webArticle.Data.Digest;
                AuthorTextBlock.Text = webArticle.Data.Author;
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            RandomArticleInit();
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", MainContent);
            MainPage.Current.MainFrame.Navigate(typeof(ReadPage), WebArticle, new SuppressNavigationTransitionInfo());
        }
    }
}
