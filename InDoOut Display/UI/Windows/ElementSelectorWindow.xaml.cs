using InDoOut_Display.UI.Controls.Screens;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class ElementSelectorWindow : Window
    {
        private IScreenItem _associatedScreen = null;

        public IScreenItem AssociatedScreen { get => _associatedScreen; set => ChangeScreen(value); }

        public ElementSelectorWindow()
        {
            InitializeComponent();
        }

        public ElementSelectorWindow(IScreenItem screen) : this()
        {
            AssociatedScreen = screen;
        }

        private void ChangeScreen(IScreenItem screen)
        {
            _associatedScreen = screen;
            ElementSelector_Main.AssociatedScreen = screen;
        }
    }
}
