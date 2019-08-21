using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIConnection : UserControl, IUIConnection
    {
        private IUIConnectionEnd _end = null;
        private IUIConnectionStart _start = null;

        public bool Hidden { get => Visibility != Visibility.Visible; set => SetHidden(value); }

        public IUIConnectionEnd AssociatedEnd { get => _end; set => SetEnd(value); }
        public IUIConnectionStart AssociatedStart { get => _start; set => SetStart(value); }

        public Point Start { get => Figure_Start.StartPoint; set => SetStart(value); }
        public Point End { get => Segment_Curve.Point3; set => SetEnd(value); }

        public UIConnection()
        {
            InitializeComponent();
        }

        public void UpdatePositionFromInputOutput(IElementDisplay display)
        {
            if (display != null && AssociatedEnd != null && AssociatedStart != null)
            {
                if (AssociatedEnd is FrameworkElement inputElement && AssociatedStart is FrameworkElement outputElement)
                {
                    Start = display.GetBestSide(outputElement, inputElement);
                    End = display.GetBestSide(inputElement, outputElement);

                    if (AssociatedStart != null && AssociatedEnd != null)
                    {
                        AssociatedStart.PositionUpdated(display.GetPosition(outputElement));
                        AssociatedEnd.PositionUpdated(display.GetPosition(inputElement));
                    }
                }
            }
        }

        private void SetEnd(IUIConnectionEnd input)
        {
            _end = input;
        }

        private void SetStart(IUIConnectionStart output)
        {
            _start = output;
        }

        private void SetEnd(Point point)
        {
            Segment_Curve.Point3 = point;

            RecalculateBezier();
        }

        private void SetStart(Point point)
        {
            Figure_Start.StartPoint = point;

            RecalculateBezier();
        }

        private void SetHidden(bool hidden)
        {
            var storyboard = new Storyboard();

            if (hidden)
            {
                var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
                animation.Completed += (sender, e) => Visibility = Visibility.Hidden;

                storyboard.Children.Add(animation);
            }
            else
            {
                storyboard.BeginTime = TimeSpan.FromMilliseconds(400);
                storyboard.Children.Add(new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)));

                Visibility = Visibility.Visible;
            }

            Storyboard.SetTarget(storyboard, this);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(OpacityProperty));

            storyboard.Begin();
        }

        private void RecalculateBezier()
        {
            var start = Start;
            var end = End;
            var difference = new Point(end.X - start.X, end.Y - start.Y);

            Segment_Curve.Point1 = new Point(end.X - (difference.X / 2d), start.Y);
            Segment_Curve.Point2 = new Point(end.X - (difference.X / 2d), end.Y);
        }
    }
}
