using Project_GuanZhi.Controls;
using Project_GuanZhi.Models;
using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
                else if (navigateType == SideMenuType.Search)
                {
                    SearchArticleInit();
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

        private void SearchArticleInit()
        {
            WaittingRing.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;
            RandomButton.Visibility = Visibility.Collapsed;
            DateSearchTextBox.Visibility = Visibility.Visible;
        }

        private void UserInterfaceInit(WebArticleModel webArticle)
        {
            if (webArticle == null)
            {
                new PopToast("数据异常！").ShowPopup();
                return;
            }
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
            var article = new AppArticleModel(WebArticle);
            MainPage.Current.AddRecentArticle(article);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", ArticleContainer);
            MainPage.Current.MainFrame.Navigate(typeof(ReadPage), WebArticle, new SuppressNavigationTransitionInfo());
        }

        private async void DateSearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string date = DateSearchTextBox.Text.Trim();
                if (string.IsNullOrEmpty(date))
                {
                    new PopToast("日期不能为空哟").ShowPopup();
                    return;
                }
                Regex dateReg = new Regex("^((((([1|2])[0-9]{1})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[13579][26])00))0229)|([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[1][0-9]|2[0-8]))))$");
                if (dateReg.IsMatch(date))
                {
                    var parseDate = AppTools.GetDateTime(date);
                    if(parseDate<new DateTime(2011,3,8))
                    {
                        new PopToast("最远只能到2011年3月8日哦~").ShowPopup();
                    }
                    else if(parseDate>DateTime.Now)
                    {
                        new PopToast("软件尚未提供预知能力哟~").ShowPopup();
                    }
                    else
                    {
                        WaittingRing.Visibility = Visibility.Visible;
                        MainContent.Visibility = Visibility.Collapsed;
                        var webArticle = await AppTools.GetAsyncJson<WebArticleModel>(AppTools.SOMEDAY_URL+date);
                        UserInterfaceInit(webArticle);
                    }
                }
                else
                {
                    var dialog = new ConfirmDialog("日期提醒", "日期的输入是有一定格式的：\n比如说要查找2018年1月1日的文章，则应输入-\n20180101\n\n输入201811或者2018/1/1这些都是不行的哟~", "");
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
