using InDoOut_Core.Entities.Functions;
using InDoOut_Display.Actions;
using InDoOut_UI_Common.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenConnections : UserControl, IScreenConnections
    {
        private readonly ActionHandler _actionHandler = null;

        public IScreen CurrentScreen => ScreenItem_Overview;

        public ScreenConnections()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenConnectionsRestingAction(ScreenItem_Overview));
        }

        public bool AddFunction(IFunction function)
        {
            if (function != null)
            {
                var functionContainer = new TextBlock
                {
                    Text = function.SafeName,
                    FontSize = 50,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                };

                Canvas_Content.Children.Add(functionContainer);
                Canvas.SetLeft(functionContainer, 200);
                Canvas.SetTop(functionContainer, 200);

                return true;
            }

            return false;
        }

        public bool RemoveFunction(IFunction function)
        {
            //Todo: Make the remove function work.

            return false;
        }

        public Point GetMousePosition()
        {
            return Mouse.GetPosition(this);
        }

        public FrameworkElement GetElementUnderMouse()
        {
            return GetElementAtPoint(GetMousePosition());
        }

        public FrameworkElement GetElementAtPoint(Point point)
        {
            return GetElementsAtPoint(point).FirstOrDefault();
        }

        public List<FrameworkElement> GetElementsUnderMouse()
        {
            return GetElementsAtPoint(GetMousePosition());
        }

        public List<FrameworkElement> GetElementsAtPoint(Point point)
        {
            var hits = new List<FrameworkElement>();

            VisualTreeHelper.HitTest(Canvas_Content, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            hits.AddRange(new List<FrameworkElement>() { Canvas_Content, this });
            return hits;
        }

        public T GetFirstElementOfType<T>(FrameworkElement element) where T : class
        {
            if (element != null)
            {
                if (typeof(T).IsAssignableFrom(element.GetType()) && element is T converted)
                {
                    return converted;
                }
                else
                {
                    var parent = VisualTreeHelper.GetParent(element);
                    return GetFirstElementOfType<T>(parent as FrameworkElement);
                }
            }

            return null;
        }

        public T GetFirstElementOfType<T>(List<FrameworkElement> elements) where T : class
        {
            foreach (var element in elements)
            {
                var foundElement = GetFirstElementOfType<T>(element);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        private HitTestFilterBehavior FilterHit(DependencyObject potentialHitTestTarget)
        {
            return potentialHitTestTarget is UIElement uiElement && uiElement.Visibility != Visibility.Visible
                ? HitTestFilterBehavior.ContinueSkipSelfAndChildren
                : HitTestFilterBehavior.Continue;
        }

        private HitTestResultBehavior NewHit(HitTestResult result, List<FrameworkElement> hits)
        {
            if (result.VisualHit != null && result.VisualHit is FrameworkElement element)
            {
                hits.Add(element);
            }

            return HitTestResultBehavior.Continue;
        }

        private void Scroll_Content_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var handled = false;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                handled = _actionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                handled = _actionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
            else
            {
                handled = _actionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement)) ?? false;
            }

            e.Handled = handled;
        }

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyDown(e.Key) ?? false;
        }

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyUp(e.Key) ?? false;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (ActualHeight / 2d));
        }
    }
}
