using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIConnection : UserControl, IUIConnection
    {
        private bool _startIsLeft = true;
        private bool _endIsLeft = true;
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

        public bool CanDelete(IBlockView blockView) => true;
        public bool CanSelect(IBlockView view) => true;

        public void Deleted(IBlockView blockView)
        {
            if (AssociatedEnd != null && AssociatedStart != null)
            {
                //Todo: Abstract this out to remove specialisations
                if (AssociatedStart is IUIOutput output && AssociatedEnd is IUIInput input)
                {
                    _ = output.AssociatedOutput?.Disconnect(input.AssociatedInput) ?? false;
                }
                else if (AssociatedStart is IUIResult result && AssociatedEnd is IUIProperty property)
                {
                    _ = result.AssociatedResult?.Disconnect(property.AssociatedProperty) ?? false;
                }
            }

            if (blockView != null)
            {
                blockView.Remove(this as IUIConnection);
            }
        }

        public void SelectionStarted(IBlockView view)
        {
            Stroke_Highlight.Visibility = Visibility.Visible;
        }

        public void SelectionEnded(IBlockView view)
        {
            Stroke_Highlight.Visibility = Visibility.Hidden;
        }

        public void UpdatePositionFromInputOutput(IElementDisplay display)
        {
            if (display != null && AssociatedEnd != null && AssociatedStart != null)
            {
                if (AssociatedEnd is FrameworkElement inputElement && AssociatedStart is FrameworkElement outputElement)
                {
                    Start = display.GetBestSide(outputElement, inputElement);
                    End = display.GetBestSide(inputElement, outputElement);

                    _startIsLeft = Start.X <= display.GetPosition(outputElement).X;
                    _endIsLeft = End.X <= display.GetPosition(inputElement).X;

                    if (AssociatedStart != null && AssociatedEnd != null)
                    {
                        AssociatedStart.PositionUpdated(display.GetPosition(outputElement));
                        AssociatedEnd.PositionUpdated(display.GetPosition(inputElement));
                    }

                    RecalculateBezier();
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

            _endIsLeft = point.X > Start.X;

            RecalculateBezier();
        }

        private void SetStart(Point point)
        {
            Figure_Start.StartPoint = point;

            _startIsLeft = point.X > End.X;

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
            var differenceStart = new Point(difference.X, difference.Y);
            var differenceEnd = new Point(difference.X, difference.Y);
            var padLimit = 150;

            if (difference.X <= padLimit && difference.X >= -padLimit)
            {
                differenceStart.X = _startIsLeft ? -padLimit : padLimit;
                differenceEnd.X = _endIsLeft ? padLimit : -padLimit;
            }

            Segment_Curve.Point1 = new Point(start.X + (differenceStart.X / 2d), start.Y);
            Segment_Curve.Point2 = new Point(end.X - (differenceEnd.X / 2d), end.Y);
        }
    }
}
