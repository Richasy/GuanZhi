﻿using Project_GuanZhi.Controls;
using Project_GuanZhi.Models;
using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Project_GuanZhi
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page,INotifyPropertyChanged
    {
        public ObservableCollection<SideMenuModel> SideMenuCollection = new ObservableCollection<SideMenuModel>();
        public ObservableCollection<AppArticleModel> RecentArticleCollection = new ObservableCollection<AppArticleModel>();
        public List<AppArticleModel> FavouriteArticleCollection = new List<AppArticleModel>();
        public List<AppArticleModel> ReadArticleCollection = new List<AppArticleModel>();
        public static MainPage Current;
        private ElementTheme _mainPageTheme;
        public ElementTheme MainPageTheme
        {
            get { return _mainPageTheme; }
            set { _mainPageTheme = value;OnPropertyChanged(); }
        }
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            MainPageInit();
        }

        private async void MainPageInit()
        {
            MainPageTheme = App.Current.RequestedTheme==ApplicationTheme.Light?ElementTheme.Light:ElementTheme.Dark;
            var image = new BitmapImage();
            image.UriSource = new Uri($"ms-appx:///Assets/{MainPageTheme.ToString()}.png");
            AppIcon.Source = image;
            var sideMenuItems = SideMenuModel.GetSideMenuList();
            foreach (var menuItem in sideMenuItems)
            {
                SideMenuCollection.Add(menuItem);
            }
            var sourceLocalRecentArticleList = await IOTools.GetLocalRecentArticleList();
            FavouriteArticleCollection = await IOTools.GetLocalFavouriteArticleList();
            ReadArticleCollection = await IOTools.GetLocalReadArticleList();
            foreach (var article in sourceLocalRecentArticleList)
            {
                RecentArticleCollection.Add(article);
            }
            TopTitleTextBlock.Text = "每日一文";
            SideMenuListView.SelectedIndex = 0;
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
            RecentListView.SelectedIndex = -1;
            switch (selectItem.Type)
            {
                case SideMenuType.Today:
                    MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Today);
                    break;
                case SideMenuType.Random:
                    MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Random);
                    break;
                case SideMenuType.Search:
                    MainFrame.Navigate(typeof(Pages.StartPage), SideMenuType.Search);
                    break;
                case SideMenuType.Favourite:
                    MainFrame.Navigate(typeof(Pages.FavouritePage));
                    break;
                case SideMenuType.About:
                    MainFrame.Navigate(typeof(Pages.AboutPage));
                    break;
                default:
                    break;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double recentRowHeight = RecentArticleRow.ActualHeight;
            RecentListView.Height = recentRowHeight - 80;
            if (width > 1000)
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
            RecentListView.SelectedIndex = -1;
            foreach (var menuItem in SideMenuCollection)
            {
                if(menuItem.Type==SideMenuType.Random)
                {
                    if(!menuItem.IsSelect)
                    {
                        menuItem.IsSelect = true;
                        TopTitleTextBlock.Text = menuItem.Title;
                        break;
                    }  
                }
                else
                {
                    menuItem.IsSelect = false;
                }
            }
        }

        public async void AddRecentArticle(AppArticleModel article)
        {
            if (RecentArticleCollection.Count > 0)
            {
                if (RecentArticleCollection.First().Equals(article))
                {
                    return;
                }
                RecentArticleCollection.Remove(RecentArticleCollection.Where(p => p.Date == article.Date).FirstOrDefault());
                if (RecentArticleCollection.Count >= 10)
                {
                    RecentArticleCollection.Remove(RecentArticleCollection.Last());
                }
            }
            RecentArticleCollection.Insert(0, article);
            await IOTools.RewriteLocalRecentArticleList(RecentArticleCollection.ToList());
        }

        public async void AddReadArticle(AppArticleModel article)
        {
            if(!ReadArticleCollection.Any(p=>p.Date==article.Date || p.Title == article.Title))
            {
                ReadArticleCollection.Add(article);
                await IOTools.RewriteLocalReadArticleList(ReadArticleCollection.ToList());
            }
        }

        public async Task<bool> AddFavouriteArticle(AppArticleModel article)
        {
            if(FavouriteArticleCollection.Any(p=>p.Date==article.Date || p.Title == article.Title))
            {
                new PopToast("您已经收藏过该文章").ShowPopup();
            }
            else
            {
                FavouriteArticleCollection.Add(article);
                bool result = await IOTools.RewriteLocalFavouriteArticleList(FavouriteArticleCollection);
                if (result)
                {
                    new PopToast("收藏成功！").ShowPopup();
                    return true;
                }
                else
                {
                    new PopToast("收藏失败了...").ShowPopup();
                }
            }
            return false;
        }

        public async Task<bool> RemoveFavouriteArticle(string date)
        {
            var sourceArticle = FavouriteArticleCollection.Where(article => article.Date == date).FirstOrDefault();
            if (sourceArticle != null)
            {
                FavouriteArticleCollection.Remove(sourceArticle);
                bool result = await IOTools.RewriteLocalFavouriteArticleList(FavouriteArticleCollection);
                if (result)
                {
                    new PopToast("已取消收藏").ShowPopup();
                    return true;
                }
                else
                {
                    new PopToast("取消收藏时出现异常").ShowPopup();
                }
            }
            else
            {
                new PopToast("收藏列表中无此项").ShowPopup();
            }
            return false;
        }

        private void RecentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var recentItem = e.ClickedItem as AppArticleModel;
            foreach (var menuItem in SideMenuCollection)
            {
                menuItem.IsSelect = false;
            }
            AddRecentArticle(recentItem);
            MainFrame.Navigate(typeof(Pages.ReadPage), recentItem.Date);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
