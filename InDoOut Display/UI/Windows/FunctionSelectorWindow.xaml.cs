using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class FunctionSelectorWindow : Window
    {
        private ICommonProgramDisplay _programDisplay = null;
        public ICommonProgramDisplay ProgramDisplay { get => _programDisplay; set => UpdateScreen(value); }

        public FunctionSelectorWindow()
        {
            InitializeComponent();
        }

        public FunctionSelectorWindow(ICommonProgramDisplay programDisplay) : this()
        {
            ProgramDisplay = programDisplay;
        }

        private void UpdateScreen(ICommonProgramDisplay programDisplay)
        {
            _programDisplay = programDisplay;
            FunctionSelector_Main.ProgramDisplay = programDisplay;
        }
    }
}
