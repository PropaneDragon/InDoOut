using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System;

namespace InDoOut_Display_Basic_Plugins.Media
{
    public class VideoPlayerElementFunction : ElementFunction
    {
        private IProperty<string> _mediaPath;
        private IProperty<double> _volumePercentage;

        public double MediaVolumePercentage => Math.Clamp(_volumePercentage?.FullValue ?? 100, 0, 100);

        public string MediaPath => _mediaPath?.FullValue;

        public override string Description => "An element capable of playing video and audio files.";

        public override string Name => "Video player";

        public override string Group => "Media";

        public override string[] Keywords => new[] { "wmv", "mp4", "mp3", "youtube", "video", "audio", "sound", "pictures", "movie", "film" };

        public VideoPlayerElementFunction()
        {
            _mediaPath = AddProperty(new Property<string>("Media path", "The path to the media to be played", true, ""));
            _volumePercentage = AddProperty(new Property<double>("Volume percentage (0 - 100)", "The volume of the media to be played", false, 100));
        }

        public override IDisplayElement CreateAssociatedUIElement()
        {
            return new VideoPlayerUIElement(this);
        }
    }
}
