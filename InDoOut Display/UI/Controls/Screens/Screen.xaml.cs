﻿using InDoOut_Core.Entities.Programs;
using InDoOut_Display.Actions;
using InDoOut_Display.Actions.Selecting;
using InDoOut_Display.Creation;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Creation;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.Removal;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class Screen : UserControl, IScreen
    {
        private ProgramViewMode _currentViewMode = ProgramViewMode.IO;

        public Size TotalSize => new(Width, Height);
        public Size ViewSize => TotalSize;
        public Point TopLeftViewCoordinate => new(0, 0);
        public Point BottomRightViewCoordinate => new(TotalSize.Width, TotalSize.Height);
        public Point CentreViewCoordinate => new(BottomRightViewCoordinate.X / 2d, BottomRightViewCoordinate.Y / 2d);
        public ProgramViewMode CurrentViewMode { get => _currentViewMode; set => ChangeMode(value); }
        public IProgram AssociatedProgram { get; set; } = null;
        public IScreenConnections AssociatedScreenConnections { get; set; } = null;
        public IDisplayElementCreator DisplayElementCreator { get; private set; } = null;
        public IDeletableRemover DeletableRemover { get; private set; } = null;
        public IActionHandler ActionHandler { get; private set; } = null;
        public ISelectionManager<ISelectable> SelectionManager { get; private set; } = null;
        public List<FrameworkElement> Elements => Grid_Elements.Children.Cast<FrameworkElement>().ToList();

        public Screen()
        {
            InitializeComponent();

            ActionHandler = new ActionHandler(new ScreenRestingAction(this));
            SelectionManager = new ScreenSelectionManager(this);
            DeletableRemover = new BasicDeletableRemover(this);
            DisplayElementCreator = new DisplayElementCreator(this);
        }

        public bool Clear()
        {
            Grid_Elements.Children.Clear();

            return true;
        }

        public void Add(FrameworkElement element)
        {
            if (element != null)
            {
                var container = AttachToContainer(element);
                if (container != null)
                {
                    _ = SelectionManager?.Set(container);
                    _ = Grid_Elements.Children.Add(container as FrameworkElement);
                }
            }
        }

        public void Add(FrameworkElement element, Point position, int zIndex = 0)
        {
            Add(element);
            SetPosition(element, position);
        }

        public void Remove(FrameworkElement element)
        {
            if (element != null)
            {
                Grid_Elements.Children.Remove(element);
            }
        }

        public void SetPosition(FrameworkElement element, Point position)
        {
            /* We do nothing. The containers position themselves as they're 
             * set as percentages as part of their movement/resize methods. */
        }

        public Point GetPosition(FrameworkElement element) => new();

        public Point GetMousePosition() => Mouse.GetPosition(this);

        public Size GetSize(FrameworkElement element) => new();

        public FrameworkElement GetElementUnderMouse() => GetElementAtPoint(GetMousePosition());

        public FrameworkElement GetElementAtPoint(Point point) => GetElementsAtPoint(point).FirstOrDefault();

        public List<FrameworkElement> GetElementsUnderMouse() => GetElementsAtPoint(GetMousePosition());

        public List<FrameworkElement> GetElementsAtPoint(Point point)
        {
            var hits = new List<FrameworkElement>();

            VisualTreeHelper.HitTest(Grid_Elements, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            hits.AddRange(new List<FrameworkElement>() { Grid_Elements, this });
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

        public ScreenEdge GetCloseEdge(Point point, double distance = 5d)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = inBounds && PointWithin(point.X, -distance, distance);
            var nearTop = inBounds && PointWithin(point.Y, -distance, distance);
            var nearRight = inBounds && PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = inBounds && PointWithin(point.Y, size.Height - distance, size.Height + distance);

            if (nearLeft)
            {
                return nearTop ? ScreenEdge.TopLeft : nearBottom ? ScreenEdge.BottomLeft : ScreenEdge.Left;
            }
            else if (nearRight)
            {
                return nearTop ? ScreenEdge.TopRight : nearBottom ? ScreenEdge.BottomRight : ScreenEdge.Right;
            }
            else if (nearBottom)
            {
                return ScreenEdge.Bottom;
            }
            else if (nearTop)
            {
                return ScreenEdge.Top;
            }

            return ScreenEdge.None;
        }

        public bool PointCloseToScreenItemEdge(Point point, double distance = 5d) => GetCloseEdge(point, distance) != ScreenEdge.None;

        private IStaticMarginElementContainer AttachToContainer(FrameworkElement element)
        {
            if (element is IStaticMarginElementContainer container)
            {
                return container;
            }

            if (element is IDisplayElement displayElement)
            {
                return new DisplayElementContainer(displayElement) { DisplayMode = CurrentViewMode == ProgramViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables };
            }
            else if (element != null)
            {
                return new StaticMarginElementContainer(element);
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

        private bool PointWithin(double point, double min, double max) => point > min && point < max;

        private void ChangeMode(ProgramViewMode mode)
        {
            if (mode != _currentViewMode)
            {
                _currentViewMode = mode;

                var elements = Elements;

                foreach (var element in elements)
                {
                    if (element is IDisplayElementContainer elementContainer)
                    {
                        elementContainer.DisplayMode = mode == ProgramViewMode.Variables ? UIFunctionDisplayMode.Variables : UIFunctionDisplayMode.IO;
                    }
                }
            }
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                _ = Keyboard.Focus(this);
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = ActionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                _ = ActionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
            else
            {
                _ = ActionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement)) ?? false;
            }

            e.Handled = false;
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyDown(e.Key) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyUp(e.Key) ?? false;

            e.Handled = false;
        }
    }
}
