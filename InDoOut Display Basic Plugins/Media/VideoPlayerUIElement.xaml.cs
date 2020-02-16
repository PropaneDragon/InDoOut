using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System;

namespace InDoOut_Display_Basic_Plugins.Media
{
    public partial class VideoPlayerUIElement : DisplayElement
    {
        public VideoPlayerUIElement(IElementFunction function) : base(function)
        {
            InitializeComponent();
        }

        protected override bool UpdateRequested(IElementFunction function)
        {
            if (function is VideoPlayerElementFunction elementFunction)
            {
                Media_VideoPlayer.Source = new Uri(elementFunction.MediaPath);
                Media_VideoPlayer.Volume = Math.Clamp(elementFunction.MediaVolumePercentage, 0, 100) / 100d;

                return true;
            }

            return false;
        }
    }
}
