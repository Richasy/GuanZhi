using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace Project_GuanZhi.Controls
{
    public sealed partial class ConfirmDialog : ContentDialog,INotifyPropertyChanged
    {
        public ConfirmDialog()
        {
            this.InitializeComponent();
            if (MainPage.Current != null)
            {
                RequestedTheme = MainPage.Current.MainPageTheme;
            }
            else
            {
                RequestedTheme = ElementTheme.Light;
            }
        }

        public ConfirmDialog(string title,string tip,string primaryButtonText="确认",string secondaryButtonText="",string closeButtonText="取消")
        {
            this.InitializeComponent();
            if (MainPage.Current != null)
            {
                RequestedTheme = MainPage.Current.MainPageTheme;
            }
            else
            {
                RequestedTheme = ElementTheme.Light;
            }
            Title = title;
            TipTextBlock.Text = tip;
            PrimaryButtonText = primaryButtonText;
            SecondaryButtonText = secondaryButtonText;
            CloseButtonText = closeButtonText;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
