using Project_GuanZhi.Models;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Project_GuanZhi.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FavouritePage : Page
    {
        private ObservableCollection<AppArticleModel> FavouriteArticleCollection = new ObservableCollection<AppArticleModel>();
        private string selectDate = "";
        public FavouritePage()
        {
            this.InitializeComponent();
            FavouriteInit();
        }

        private void FavouriteInit()
        {
            if (MainPage.Current.FavouriteArticleCollection.Count > 0)
            {
                EmptyTipTextBlock.Visibility = Visibility.Collapsed;
                foreach (var article in MainPage.Current.FavouriteArticleCollection)
                {
                    FavouriteArticleCollection.Add(article);
                }
            }
        }
        private void MenuFlyout(object sender)
        {
            var grid = (Grid)sender;
            selectDate = grid.Name;
            FlyoutBase.ShowAttachedFlyout(grid);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            MenuFlyout(sender);
        }

        

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            MenuFlyout(sender);
        }

        private async void ArticleMenuFlyout_Handle(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectDate))
            {
                var result = await MainPage.Current.RemoveFavouriteArticle(selectDate);
                if (result)
                {
                    FavouriteArticleCollection.Remove(FavouriteArticleCollection.Where(p => p.Date == selectDate).FirstOrDefault());
                    selectDate = "";
                }
            }
            
        }

        private void FavouriteGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var article = (AppArticleModel)e.ClickedItem;
            MainPage.Current.MainFrame.Navigate(typeof(ReadPage), article.Date);
        }
    }
}
