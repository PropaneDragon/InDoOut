using InDoOut_Display.UI.Controls.Screens;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class ElementSelectorWindow : Window
    {
        private IScreen _associatedScreen = null;

        public IScreen AssociatedScreen { get => _associatedScreen; set => ChangeScreen(value); }

        public ElementSelectorWindow()
        {
            InitializeComponent();
        }

        public ElementSelectorWindow(IScreen screen) : this()
        {
            AssociatedScreen = screen;
        }

        private void ChangeScreen(IScreen screen)
        {
            _associatedScreen = screen;
            ElementSelector_Main.AssociatedScreen = screen;
        }
    }
}
