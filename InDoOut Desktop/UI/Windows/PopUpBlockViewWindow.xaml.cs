using InDoOut_Core.Entities.Programs;
using System.Windows;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class PopUpBlockViewWindow : Window
    {
        public IProgram AssociatedProgram { get => BlockView_Main.AssociatedProgram; set => ChangeProgram(value); }

        public PopUpBlockViewWindow()
        {
            InitializeComponent();
        }

        public PopUpBlockViewWindow(IProgram program) : this()
        {
            AssociatedProgram = program;
        }

        private void ChangeProgram(IProgram program)
        {
            BlockView_Main.AssociatedProgram = program;

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            var programName = "No program";
            var program = AssociatedProgram;

            if (program != null)
            {
                programName = string.IsNullOrEmpty(program.Name) ? "Untitled" : program.Name;
            }

            Title = $"{programName} (Preview)";
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
