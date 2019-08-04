using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIProperty : UserControl, IUIProperty
    {
        private IProperty _property = null;

        public IProperty AssociatedProperty { get => _property; set => SetProperty(value); }

        public UIProperty() : base()
        {
            InitializeComponent();
        }

        public UIProperty(IProperty property) : this()
        {
            AssociatedProperty = property;
        }

        private void SetProperty(IProperty property)
        {
            if (_property != null)
            {
                //Todo: Teardown old property
            }

            _property = property;

            if (_property != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _property.Name;
            }
        }
    }
}
