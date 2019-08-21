using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIResult : UserControl, IUIResult
    {
        private IResult _result = null;

        public IResult AssociatedResult { get => _result; set => SetResult(value); }

        public UIResult() : base()
        {
            InitializeComponent();
        }

        public UIResult(IResult result) : this()
        {
            AssociatedResult = result;
        }

        public void PositionUpdated(Point position)
        {
            if (AssociatedResult != null)
            {
            }
        }

        private void SetResult(IResult result)
        {
            if (_result != null)
            {
                //Todo: Teardown old property
            }

            _result = result;

            if (_result != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _result.Name;
            }
        }
    }
}
