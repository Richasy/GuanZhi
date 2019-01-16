using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
using Microsoft.Xaml.Interactivity;
using Project_GuanZhi.Models;
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
    public sealed partial class ReadPage : Page
    {
        public ReadPage()
        {
            this.InitializeComponent();
            Interaction.GetBehaviors(ReadScrollView).Add(new FadeHeaderBehavior { HeaderElement = HeaderContainer });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter!=null && e.Parameter is WebArticleData)
            {
                var sourceData = e.Parameter as WebArticleData;
                
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

        }
    }
}
