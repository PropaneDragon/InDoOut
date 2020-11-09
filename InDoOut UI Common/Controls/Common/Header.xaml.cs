using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.Common
{
    public partial class Header : UserControl
    {
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Header), new PropertyMetadata("Title"));
        public static DependencyProperty SubtitleProperty = DependencyProperty.Register("Subtitle", typeof(string), typeof(Header));

        public string Title { get => GetValue(TitleProperty) as string; set => SetValue(TitleProperty, value); }
        public string Subtitle { get => GetValue(SubtitleProperty) as string; set => SetValue(SubtitleProperty, value); }

        public Header()
        {
            InitializeComponent();
        }
    }
}
