using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IDraggable
    {
        public IFunction Function { get; set; } = null;

        public UIFunction()
        {
            InitializeComponent();
        }

        public UIFunction(IFunction function)
        {
            Function = function;
        }

        public bool CanDrag()
        {
            return true;
        }

        public void DragStarted()
        {
        }

        public void DragMoved()
        {
        }

        public void DragEnded()
        {
        }
    }
}
