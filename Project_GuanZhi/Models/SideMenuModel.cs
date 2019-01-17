using Project_GuanZhi.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Project_GuanZhi.Models
{
    public class SideMenuModel : INotifyPropertyChanged
    {
        private Visibility _signVisibility;
        private bool _isSelect;
        private FontWeight _titleFontWeight;
        private Brush _titleForeground;
        public int Id { get; set; }
        public SideMenuType Type { get; set; }
        public string Title { get; set; }
        public bool IsSelect
        {
            get { return _isSelect; }
            set { _isSelect = value; SelectChanged(value); }
        }
        public Visibility SignVisibility
        {
            get { return _signVisibility; }
            set { _signVisibility = value; OnPropertyChanged(); }
        }
        public FontWeight TitleFontWeight
        {
            get { return _titleFontWeight; }
            set { _titleFontWeight = value; OnPropertyChanged(); }
        }
        public Brush TitleForeground
        {
            get { return _titleForeground; }
            set { _titleForeground = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 获取项目侧边栏菜单列表，默认选中笔记列表
        /// </summary>
        /// <param name="defaultSelectItemType">默认选中项类型</param>
        /// <returns></returns>
        public static List<SideMenuModel> GetSideMenuList(SideMenuType defaultSelectItemType = SideMenuType.Today)
        {
            var result = new List<SideMenuModel>()
            {
                new SideMenuModel(0,SideMenuType.Today),
                new SideMenuModel(1,SideMenuType.Random),
                new SideMenuModel(2,SideMenuType.Search),
                new SideMenuModel(3,SideMenuType.Favourite),
                new SideMenuModel(4,SideMenuType.About)
            };
            foreach (var item in result)
            {
                if (item.Type == defaultSelectItemType)
                {
                    item.IsSelect = true;
                }
            }
            return result;
        }

        public SideMenuModel() { }

        public SideMenuModel(int id, SideMenuType type, bool isSelect = false)
        {
            Id = id;
            Type = type;
            var content = GetSideMenuItemContentByType(type);
            Title = content;
            IsSelect = isSelect;
            SelectChanged(isSelect);
        }

        public void UpdateLayout()
        {
            TitleForeground = IsSelect ? AppTools.GetThemeSolidColorBrush("ImportantTextColor") : AppTools.GetThemeSolidColorBrush("SubTextColor");
        }

        private string GetSideMenuItemContentByType(SideMenuType cla)
        {
            string title = "";
            switch (cla)
            {
                case SideMenuType.Today:
                    title = "每日一文";
                    break;
                case SideMenuType.Random:
                    title = "随机好文";
                    break;
                case SideMenuType.Search:
                    title = "随性查找";
                    break;
                case SideMenuType.Favourite:
                    title = "文章收藏";
                    break;
                case SideMenuType.About:
                    title = "关于软件";
                    break;
                default:
                    break;
            }
            return title;
        }

        private void SelectChanged(bool isSelect)
        {
            SignVisibility = isSelect ? Visibility.Visible : Visibility.Collapsed;
            TitleFontWeight = isSelect ? FontWeights.Bold : FontWeights.Normal;
            TitleForeground = IsSelect ? AppTools.GetThemeSolidColorBrush("ImportantTextColor") : AppTools.GetThemeSolidColorBrush("SubTextColor");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            var item = obj as SideMenuModel;
            return item != null &&
                   Type == item.Type;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }
    }
}
