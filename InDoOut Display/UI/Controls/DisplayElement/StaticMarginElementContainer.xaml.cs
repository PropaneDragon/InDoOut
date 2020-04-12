using InDoOut_Display_Core.Actions.Resizing;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class StaticMarginElementContainer : UserControl, IStaticMarginElementContainer
    {
        public FrameworkElement ContainedElement { get => ContentPresenter_Element.Content as FrameworkElement; set => ContentPresenter_Element.Content = value; }

        public StaticMarginElementContainer()
        {
            InitializeComponent();
        }

        public StaticMarginElementContainer(FrameworkElement element) : this()
        {
            ContainedElement = element;
        }

        public Thickness MarginPercentages { get => GetMarginPercentages(); set => SetMarginPercentages(value); }

        public Size Size => new Size(Border_Presenter.ActualWidth, Border_Presenter.ActualHeight);

        public virtual bool AutoScale { get; set; } = false;
        public virtual double Scale { get; set; } = 1d;

        public virtual bool CanDrag(IElementDisplay view) => false;

        public virtual bool CanResize(IScreen screen) => false;

        public virtual bool CanScale(IScreen screen) => false;

        public virtual bool CanSelect(IElementDisplay view) => false;

        public virtual bool CloseToEdge(IScreen screen, Point point, double distance = 5) => false;

        public virtual void DragEnded(IElementDisplay view) { }

        public virtual void DragMoved(IElementDisplay view, Point delta) { }

        public virtual void DragStarted(IElementDisplay view) { }

        public virtual ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5) => ResizeEdge.None;

        public virtual void ResizeEnded(IScreen screen) { }

        public virtual void ResizeMoved(IScreen screen, ResizeEdge edge, Point delta) { }

        public virtual void ResizeStarted(IScreen screen) { }

        public virtual void ScaleChanged(IScreen screen) { }

        public virtual void SelectionEnded(IElementDisplay view) { }

        public virtual void SelectionStarted(IElementDisplay view) { }

        private void SetMarginPercentages(Thickness thickness)
        {
            Column_Width_Left.Width = new GridLength(thickness.Left, GridUnitType.Star);
            Column_Width_Right.Width = new GridLength(thickness.Right, GridUnitType.Star);
            Row_Height_Above.Height = new GridLength(thickness.Top, GridUnitType.Star);
            Row_Height_Below.Height = new GridLength(thickness.Bottom, GridUnitType.Star);
        }

        private Thickness GetMarginPercentages() => new Thickness(Column_Width_Left.Width.Value, Row_Height_Above.Height.Value, Column_Width_Right.Width.Value, Row_Height_Below.Height.Value);
    }
}
