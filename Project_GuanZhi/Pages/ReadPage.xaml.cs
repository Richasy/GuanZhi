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
        public FontFamily ReadFontFamily
        {
            get { return _readFontFamily; }
            set { _readFontFamily = value; OnPropertyChanged(); }
        }
        private bool isToolBarHide = false;
        private ObservableCollection<SystemFont> SystemFontCollection = new ObservableCollection<SystemFont>();
        public ReadPage()
        {
            this.InitializeComponent();
            Interaction.GetBehaviors(ReadScrollView).Add(new FadeHeaderBehavior { HeaderElement = HeaderContainer });
            ReadScrollView.AddHandler(PointerWheelChangedEvent, new PointerEventHandler(ReadScrollView_PointerWheelChanged), true);
            var fontList = SystemFont.GetFonts();
            foreach (var font in fontList)
            {
                SystemFontCollection.Add(font);
            }
            string saveFontFamily = AppTools.GetLocalSetting(AppSettings.ReadFontFamily, "");
            if (string.IsNullOrEmpty(saveFontFamily))
            {
                ReadFontFamily=(FontFamily)Application.Current.Resources["NormalFont"];
            }
            else
            {
                ReadFontFamily = new FontFamily(saveFontFamily);
            }
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
                paragraph.TextIndent = 34;
                paragraph.LineHeight = 35;
                paragraph.Margin = new Thickness(0, 0, 0, 25);
                ReadTextBlock.Blocks.Add(paragraph);
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double pageWidth = e.NewSize.Width;
            if (windowWidth > 768)
            {
                ArticleContainer.Padding = new Thickness(0, 30, 0, 0);
            }
            else
            {
                ArticleContainer.Padding = new Thickness(0);
            }
        }

        private void ReadScrollView_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(ReadScrollView).Properties.MouseWheelDelta < 0)
            {
                if (!isToolBarHide)
                {
                    isToolBarHide = true;
                    ToolbarHide.Begin();
                }
            }
            else
            {
                if (isToolBarHide)
                {
                    isToolBarHide = false;
                    ToolbarShow.Begin();
                }
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
    }
}
