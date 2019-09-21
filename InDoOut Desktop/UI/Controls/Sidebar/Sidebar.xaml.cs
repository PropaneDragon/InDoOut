using InDoOut_Core.Functions;
using InDoOut_Desktop.Programs;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        private bool _collapsed = false;
        private readonly TimeSpan _animationTime = TimeSpan.FromMilliseconds(500);
        private IBlockView _blockView = null;

        public bool Collapsed { get => _collapsed; set { if (value) Collapse(); else Expand(); } }
        public IBlockView BlockView { get => _blockView; set => BlockViewChanged(value); }

        public Sidebar()
        {
            InitializeComponent();
        }

        public void Collapse()
        {
            if (!_collapsed)
            {
                var offset = -(ActualWidth - ColumnDefinition_Extended.Width.Value);
                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(offset, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var fadeOutAnimation = new DoubleAnimation(0, _animationTime) { EasingFunction = easingFunction };

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, fadeOutAnimation);

                sidebarAnimation.Completed += (sender, e) => Grid_CollapsibleContent.Visibility = Visibility.Hidden;                
            }

            _collapsed = true;
        }

        public void Expand()
        {
            if (_collapsed)
            {
                Grid_CollapsibleContent.Visibility = Visibility.Visible;

                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(0, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var opacityAnimation = new DoubleAnimation(1, _animationTime) { EasingFunction = easingFunction }; ;

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, opacityAnimation);
            }

            _collapsed = false;
        }
        private void BlockViewChanged(IBlockView blockView)
        {
            _blockView = blockView;

            ItemList_Functions.FunctionView = blockView;
        }

        private void Button_Collapse_Click(object sender, RoutedEventArgs e)
        {
            Collapsed = !Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Collapsed = true;
        }

        private void SearchBar_SearchRequested(object sender, Search.SearchArgs e)
        {
            ItemList_Functions.Filter(e.Query);
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            if (_blockView != null)
            {
                _blockView.CurrentViewMode = _blockView.CurrentViewMode == BlockViewMode.IO ? BlockViewMode.Variables : BlockViewMode.IO;
            }
        }

        private void Button_RunProgram_Click(object sender, RoutedEventArgs e)
        {
            _blockView?.AssociatedProgram?.Trigger(null);
        }

        private void Button_NewProgram_Click(object sender, RoutedEventArgs e)
        {
            _ = ProgramHolder.Instance.RemoveProgram(_blockView?.AssociatedProgram);
            _blockView.AssociatedProgram = ProgramHolder.Instance.NewProgram();
        }

        private void Button_OpenProgram_Click(object sender, RoutedEventArgs e)
        {
            var program = ProgramSaveLoad.Instance.LoadProgramDialog(ProgramHolder.Instance, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
            if (program != null)
            {
                _ = ProgramHolder.Instance.RemoveProgram(_blockView?.AssociatedProgram);
                _blockView.AssociatedProgram = program;
            }
        }

        private void Button_SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            _ = ProgramSaveLoad.Instance.TrySaveProgramFromMetadata(_blockView?.AssociatedProgram, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }

        private void Button_SaveProgramAs_Click(object sender, RoutedEventArgs e)
        {
            _ = ProgramSaveLoad.Instance.SaveProgramDialog(_blockView?.AssociatedProgram, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }
    }
}
