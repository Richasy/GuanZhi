using Project_GuanZhi.Models;
using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Project_GuanZhi
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<SideMenuModel> SideMenuCollection = new ObservableCollection<SideMenuModel>();
        public ObservableCollection<AppArticleModel> RecentArticleCollection = new ObservableCollection<AppArticleModel>();
        public static MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            MainPageInit();
        }

        private async void MainPageInit()
        {
            var sideMenuItems = SideMenuModel.GetSideMenuList();
            foreach (var menuItem in sideMenuItems)
            {
                SideMenuCollection.Add(menuItem);
            }
            var sourceLocalRecentArticleList = await IOTools.GetLocalRecentArticleList();
            foreach (var article in sourceLocalRecentArticleList)
            {
                RecentArticleCollection.Add(article);
            }
            TopTitleTextBlock.Text = "每日一文";
            MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Today);
        }

        private void SideMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (var menuItem in SideMenuCollection)
            {
                menuItem.IsSelect = false;
            }
            TopMenuFlyout.Hide();
            var selectItem = e.ClickedItem as SideMenuModel;
            TopTitleTextBlock.Text = selectItem.Title;
            selectItem.IsSelect = true;
            switch (selectItem.Type)
            {
                case SideMenuType.Today:
                    MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Today);
                    break;
                case SideMenuType.Random:
                    MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Random);
                    break;
                case SideMenuType.Search:
                    break;
                case SideMenuType.Favourite:
                    break;
                default:
                    break;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            if (width > 768)
            {
                SideGrid.Visibility = Visibility.Visible;
                TopGrid.Visibility = Visibility.Collapsed;
                Grid.SetRowSpan(MainFrame, 2);
                Grid.SetColumnSpan(MainFrame, 1);
                Grid.SetColumn(MainFrame, 1);
            }
            else
            {
                SideGrid.Visibility = Visibility.Collapsed;
                TopGrid.Visibility = Visibility.Visible;
                Grid.SetRowSpan(MainFrame, 1);
                Grid.SetColumnSpan(MainFrame, 2);
                Grid.SetRow(MainFrame, 1);
            }
        }

        private void TopMenuButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        public void NextPageHandle()
        {
            foreach (var menuItem in SideMenuCollection)
            {
                if(menuItem.Type==SideMenuType.Random && menuItem.IsSelect)
                {
                    if(!menuItem.IsSelect)
                    {
                        menuItem.IsSelect = true;
                        TopTitleTextBlock.Text = menuItem.Title;
                        break;
                    }  
                }

            }
        }
    }
}
