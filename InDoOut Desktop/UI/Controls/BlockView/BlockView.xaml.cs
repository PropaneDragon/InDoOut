using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Controls.CoreEntityRepresentation;
using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : UserControl, IFunctionView
    {
        private ActionHandler _actionHandler = null;
        private IProgram _program = null;

        public IProgram Program { get => _program; set => SetProgram(value); }

        public BlockView()
        {
            InitializeComponent();
            SetProgram(new Program());

            _actionHandler = new ActionHandler(new BlockViewRestingAction(Scroll_Content));
        }

        public bool Add(IFunction function)
        {
            if (_program != null)
            {
                if (_program.AddFunction(function))
                {
                    PlaceFunction(function);

                    return true;
                }
            }

            return false;
        }

        protected void SetProgram(IProgram program)
        {
            Clear();

            if (_program != null)
            {
                //Todo: Add teardown from the previous program.
            }

            _program = program;

            if (_program != null)
            {
                PlaceFunctions(_program.Functions);
            }
        }

        private void Clear()
        {
            Canvas_Content.Children.Clear();
        }

        private void PlaceFunctions(List<IFunction> functions)
        {
            foreach (var function in functions)
            {
                PlaceFunction(function);
            }
        }

        private void PlaceFunction(IFunction function)
        {
            var uiFunction = new UIFunction(function);

            Canvas_Content.Children.Add(uiFunction);

            Canvas.SetLeft(uiFunction, 5000);
            Canvas.SetTop(uiFunction, 5000);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (Scroll_Content.ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (Scroll_Content.ActualHeight / 2d));
        }

        private void Scroll_Content_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));
        }

        private void Scroll_Content_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));
        }

        private void Scroll_Content_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _actionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));
        }
    }
}
