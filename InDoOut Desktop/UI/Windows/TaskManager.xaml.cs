using InDoOut_Desktop.Programs;
using System.Windows;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class TaskManager : Window
    {
        public IProgramHolder ProgramHolder { get; private set; } = null;

        public TaskManager(IProgramHolder programHolder)
        {
            ProgramHolder = programHolder;

            InitializeComponent();
            PopulateList();
        }

        private void PopulateList()
        {
        }
    }
}
