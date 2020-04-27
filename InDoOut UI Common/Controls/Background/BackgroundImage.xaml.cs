using System;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.Background
{
    public partial class BackgroundImage : UserControl
    {
        public enum ImageStyles { Light, Dark, Colour };

        private ImageStyles _imageStyle = ImageStyles.Light;

        public ImageStyles ImageStyle { get => _imageStyle; set => SetImageStyle(value); }

        public BackgroundImage()
        {
            InitializeComponent();

            SetImageStyle(ImageStyle);
        }

        private void SetImageStyle(ImageStyles style)
        {
            _imageStyle = style;

            SetBackgroundImage(style);
        }

        private void SetBackgroundImage(ImageStyles style)
        {
            var images = Grid_Images.Children;

            foreach (UIElement image in images)
            {
                image.Visibility = Visibility.Collapsed;
            }

            var elementToShow = GetElementForStyle(style);
            if (elementToShow != null)
            {
                elementToShow.Visibility = Visibility.Visible;
            }
        }

        private UIElement GetElementForStyle(ImageStyles style)
        {
            return style switch
            {
                ImageStyles.Colour => Image_Colour,
                ImageStyles.Dark => Image_Dark,
                ImageStyles.Light => Image_Light,
                _ => Image_Light
            };
        }
    }
}
