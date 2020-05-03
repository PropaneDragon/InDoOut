using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InDoOut_UI_Common.Animation
{
    public class BrushAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));

        public Brush From { get => GetValue(FromProperty) as Brush; set => SetValue(FromProperty, value); }
        public Brush To { get => GetValue(ToProperty) as Brush; set => SetValue(ToProperty, value); }

        public override Type TargetPropertyType => typeof(Brush);

        public object GetCurrentValue(Brush defaultOriginValue, Brush defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress.HasValue)
            {
                defaultOriginValue = From ?? defaultOriginValue;
                defaultDestinationValue = To ?? defaultDestinationValue;

                if (animationClock.CurrentProgress.Value == 0)
                {
                    return defaultOriginValue;
                }
                else if (animationClock.CurrentProgress.Value == 1)
                {
                    return defaultDestinationValue;
                }

                return new VisualBrush(new Grid()
                {
                    Children =
                    {
                        new Border()
                        {
                            Width = 1,
                            Height = 1,
                            Background = defaultOriginValue,
                            Opacity = 1 - animationClock.CurrentProgress.Value
                        },
                        new Border()
                        {
                            Width = 1,
                            Height = 1,
                            Background = defaultDestinationValue,
                            Opacity = animationClock.CurrentProgress.Value
                        }
                    }
                });
            }

            return Brushes.Transparent;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            return GetCurrentValue(defaultOriginValue as Brush, defaultDestinationValue as Brush, animationClock);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }
    }
}
