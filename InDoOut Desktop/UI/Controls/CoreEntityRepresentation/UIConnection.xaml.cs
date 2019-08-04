using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

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
            Visibility = hidden ? Visibility.Hidden : Visibility.Visible;
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
