using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class ElementSelectorWindow : Window
    {
        private ICommonProgramDisplay _associatedProgramDisplay = null;

        public ICommonProgramDisplay AssociatedProgramDisplay { get => _associatedProgramDisplay; set => ChangeScreen(value); }

        public ElementSelectorWindow()
        {
            InitializeComponent();
        }

        public ElementSelectorWindow(ICommonProgramDisplay programDisplay) : this()
        {
            AssociatedProgramDisplay = programDisplay;
        }

        private void ChangeScreen(ICommonProgramDisplay programDisplay)
        {
            _associatedProgramDisplay = programDisplay;
            ElementSelector_Main.AssociatedScreen = (programDisplay as IScreenConnections)?.CurrentScreen;
        }
    }
}
