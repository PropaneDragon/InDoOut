using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIConnection : UserControl, IUIConnection
    {
        private IUIInput _input = null;
        private IUIOutput _output = null;

        public IUIInput AssociatedInput { get => _input; set => SetInput(value); }
        public IUIOutput AssociatedOutput { get => _output; set => SetOutput(value); }

        public Point Start { get => Figure_Start.StartPoint; set => SetStart(value); }
        public Point End { get => Segment_Curve.Point3; set => SetEnd(value); }

        public UIConnection()
        {
            InitializeComponent();
        }

        private void SetInput(IUIInput input)
        {
            _input = input;
        }

        private void SetOutput(IUIOutput output)
        {
            _output = output;
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
