using InDoOut_Display.UI.Controls.Screens;
using System;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class FunctionSelectorWindow : Window
    {
        private IScreenConnections _screenConnections = null;
        public IScreenConnections ScreenConnections { get => _screenConnections; set => UpdateScreen(value); }

        public FunctionSelectorWindow()
        {
            InitializeComponent();
        }

        public FunctionSelectorWindow(IScreenConnections screenConnections) : this()
        {
            ScreenConnections = screenConnections;
        }

        private void UpdateScreen(IScreenConnections screen)
        {
            _screenConnections = screen;
            FunctionSelector_Main.Screen = screen;
        }
    }
}
