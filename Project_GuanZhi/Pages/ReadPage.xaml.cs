using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
using Microsoft.Xaml.Interactivity;
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
using Windows.UI.Xaml.Documents;
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
    public sealed partial class ReadPage : Page,INotifyPropertyChanged
    {
        private FontFamily _readFontFamily;
        private int _readFontSize;
        private double _readLineHeight;
        private double _readParagraphHeight;
        public FontFamily ReadFontFamily
        {
            get { return _readFontFamily; }
            set { _readFontFamily = value; OnPropertyChanged(); }
        }
        public int ReadFontSize
        {
            get { return _readFontSize; }
            set { _readFontSize = value;OnPropertyChanged();UpdateLayout(); }
        }
        public double ReadLineHeight
        {
            get { return _readLineHeight; }
            set { _readLineHeight = value*ReadFontSize; OnPropertyChanged();ReadLayoutUpdate(); }
        }
        public double ReadParagraphHeight
        {
            get { return _readParagraphHeight; }
            set { _readParagraphHeight = value * ReadLineHeight; OnPropertyChanged(); ReadLayoutUpdate(); }
        }
        private bool isFavourite = false;
        private bool isToolBarHide = false;
        private bool isRead = false;
        private ObservableCollection<SystemFont> SystemFontCollection = new ObservableCollection<SystemFont>();
        private AppArticleModel ThisArticleData = null;
        private double scrollViewLocation = 0;
        public ReadPage()
        {
            this.InitializeComponent();
            Interaction.GetBehaviors(ReadScrollView).Add(new FadeHeaderBehavior { HeaderElement = HeaderContainer });
            var fontList = SystemFont.GetFonts();
            foreach (var font in fontList)
            {
                SystemFontCollection.Add(font);
            }
            bool isDark = AppTools.GetLocalSetting(AppSettings.Theme, "Light") == "Dark";
            if (isDark)
                DarkModeSwitch.IsOn = true;
            else
                DarkModeSwitch.IsOn = false;
            string saveFontFamily = AppTools.GetLocalSetting(AppSettings.ReadFontFamily, "");
            if (string.IsNullOrEmpty(saveFontFamily))
            {
                ReadFontFamily=(FontFamily)Application.Current.Resources["NormalFont"];
            }
            else
            {
                ReadFontFamily = new FontFamily(saveFontFamily);
            }
            ReadFontSize = Convert.ToInt32(AppTools.GetLocalSetting(AppSettings.ReadFontSize, "16"));
            ReadLineHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadLineHeight, "1.5"));
            ReadParagraphHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadParagraphHeight, "0.5"));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter!=null)
            {
                if(e.Parameter is WebArticleData)
                {
                    var sourceData = e.Parameter as WebArticleData;
                    WebArticleHandle(sourceData);
                    
                }
                else if(e.Parameter is string)
                {
                    string date = e.Parameter as string;
                    string url = AppTools.SOMEDAY_URL + date;
                    WaittingRing.IsActive = true;
                    var webData = await AppTools.GetAsyncJson<WebArticleModel>(url);
                    WaittingRing.IsActive = false;
                    if (webData != null)
                    {
                        WebArticleHandle(webData.Data);
                    }
                }
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
                if (anim != null)
                {
                    anim.TryStart(HeaderContainer);
                }
            }
        }

        private void WebArticleHandle(WebArticleData sourceData)
        {
            string sourceContent = sourceData.Content;
            var contentSplit = sourceContent.Split("</p>", StringSplitOptions.RemoveEmptyEntries);
            ReadTextBlock.Blocks.Clear();
            ReadScrollView.ChangeView(0,0,1);
            foreach (var splitItem in contentSplit)
            {
                var paragraph = new Paragraph();
                var run = new Run();
                run.Text = splitItem.Replace("<p>", "");
                paragraph.Inlines.Add(run);
                paragraph.TextIndent = ReadFontSize*2;
                paragraph.LineHeight = ReadLineHeight;
                paragraph.Margin = new Thickness(0, 0, 0, ReadParagraphHeight);
                ReadTextBlock.Blocks.Add(paragraph);
            }
            ThisArticleData = new AppArticleModel(sourceData);
            isFavourite = MainPage.Current.FavouriteArticleCollection.Any(article => article.Date == sourceData.Date.Curr || article.Title == sourceData.Title);
            LikeButton.Content = isFavourite ? "" : "";
            isRead = MainPage.Current.ReadArticleCollection.Any(article => article.Date == sourceData.Date.Curr || article.Title == sourceData.Title);
            if(!isRead && ThisArticleData.WordCount <= 1000)
            {
                isRead = true;
                MainPage.Current.AddReadArticle(ThisArticleData);
            }
            TitleTextBlock.Text = sourceData.Title;
            AuthorTextBlock.Text = sourceData.Author;
            WordCountRun.Text = sourceData.Wc.ToString();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.NextPageHandle();
            WaittingRing.IsActive = true;
            var webData = await AppTools.GetAsyncJson<WebArticleModel>(AppTools.RANDOM_URL);
            WaittingRing.IsActive = false;
            if (webData != null)
            {
                var article = new AppArticleModel(webData.Data);
                MainPage.Current.AddRecentArticle(article);
                WebArticleHandle(webData.Data);
            }
        }

        private void ReadLayoutUpdate()
        {
            var blocks = ReadTextBlock.Blocks;
            foreach (var item in blocks)
            {
                if(item is Paragraph)
                {
                    var para = item as Paragraph;
                    para.LineHeight = ReadLineHeight;
                    para.TextIndent = ReadFontSize * 2;
                    para.Margin = new Thickness(0, 0, 0, ReadParagraphHeight);
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double pageWidth = e.NewSize.Width;
            if (windowWidth > 1000)
            {
                ArticleContainer.Padding = new Thickness(0, 30, 0, 0);
            }
            else
            {
                ArticleContainer.Padding = new Thickness(0);
            }
        }
        

        private void FontButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(FontButton);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FontListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectFont = (SystemFont)e.ClickedItem;
            ReadFontFamily = selectFont.FontFamily;
            AppTools.WriteLocalSetting(AppSettings.ReadFontFamily, selectFont.Name);
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFavourite)
            {
                bool result = await MainPage.Current.RemoveFavouriteArticle(ThisArticleData.Date);
                if (result)
                {
                    isFavourite = false;
                    LikeButton.Content = "";
                }
            }
            else
            {
                bool result = await MainPage.Current.AddFavouriteArticle(ThisArticleData);
                if (result)
                {
                    isFavourite = true;
                    LikeButton.Content = "";
                }
            }
        }

        private async void DarkModeSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (DarkModeSwitch.IsOn)
            {
                MainPage.Current.MainPageTheme = ElementTheme.Dark;
                MainPage.Current.RequestedTheme = ElementTheme.Dark;
                AppTools.WriteLocalSetting(AppSettings.Theme, "Dark");
            }
            else
            {
                MainPage.Current.MainPageTheme = ElementTheme.Light;
                MainPage.Current.RequestedTheme = ElementTheme.Light;
                AppTools.WriteLocalSetting(AppSettings.Theme, "Light");
            }
            await Task.Delay(10);
            OptionFlyout.Hide();
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(OptionButton);
        }

        private void AddFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            ReadFontSize = ReadFontSize >= 40 ? 40 : ReadFontSize + 2;
            AppTools.WriteLocalSetting(AppSettings.ReadFontSize, ReadFontSize.ToString());
        }

        private void ReduceFontSizeButton_Click(object sender, RoutedEventArgs e)
        {
            ReadFontSize = ReadFontSize <= 10 ? 10 : ReadFontSize - 2;
            AppTools.WriteLocalSetting(AppSettings.ReadFontSize, ReadFontSize.ToString());
        }

        private void AddLineHeightSizeButton_Click(object sender, RoutedEventArgs e)
        {
            double sourceLineHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadLineHeight, "1.5"));
            sourceLineHeight = sourceLineHeight >= 3 ? 3 : sourceLineHeight + 0.1;
            ReadLineHeight = sourceLineHeight;
            AppTools.WriteLocalSetting(AppSettings.ReadLineHeight, sourceLineHeight.ToString());
        }

        private void ReduceLineHeightSizeButton_Click(object sender, RoutedEventArgs e)
        {
            double sourceLineHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadLineHeight, "1.5"));
            sourceLineHeight = sourceLineHeight <= 1 ? 1 : sourceLineHeight - 0.1;
            ReadLineHeight = sourceLineHeight;
            AppTools.WriteLocalSetting(AppSettings.ReadLineHeight, sourceLineHeight.ToString());
        }
        private void AddParagraphHeightSizeButton_Click(object sender, RoutedEventArgs e)
        {
            double sourceParagraphHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadParagraphHeight, "0.5"));
            sourceParagraphHeight = sourceParagraphHeight >= 2 ? 2 : sourceParagraphHeight + 0.1;
            ReadParagraphHeight = sourceParagraphHeight;
            AppTools.WriteLocalSetting(AppSettings.ReadParagraphHeight, sourceParagraphHeight.ToString());
        }

        private void ReduceParagraphHeightSizeButton_Click(object sender, RoutedEventArgs e)
        {
            double sourceParagraphHeight = Convert.ToDouble(AppTools.GetLocalSetting(AppSettings.ReadParagraphHeight, "0.5"));
            sourceParagraphHeight = sourceParagraphHeight <= 0.1 ? 0.1 : sourceParagraphHeight - 0.1;
            ReadParagraphHeight = sourceParagraphHeight;
            AppTools.WriteLocalSetting(AppSettings.ReadParagraphHeight, sourceParagraphHeight.ToString());
        }

        private void ReadScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ReadScrollView.ScrollableHeight - ReadScrollView.VerticalOffset < 5 && !isRead)
            {
                isRead = true;
                MainPage.Current.AddReadArticle(ThisArticleData);
            }
            if (ReadScrollView.VerticalOffset != scrollViewLocation)
            {
                if (ReadScrollView.VerticalOffset > scrollViewLocation + 5)
                {
                    if (!isToolBarHide)
                    {
                        isToolBarHide = true;
                        ToolbarHide.Begin();
                    }
                }
                else if (ReadScrollView.VerticalOffset < scrollViewLocation-5)
                {
                    if (isToolBarHide)
                    {
                        isToolBarHide = false;
                        ToolbarShow.Begin();
                    }
                }
                scrollViewLocation = ReadScrollView.VerticalOffset;
            }
        }
    }
}
