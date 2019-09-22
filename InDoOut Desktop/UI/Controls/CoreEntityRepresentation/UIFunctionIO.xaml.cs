using System.Windows.Controls;
using System.Windows.Media;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunctionIO : UserControl
    {
        public enum IOType
        {
            None,
            Positive,
            Negative,
            Neutral
        }

        private readonly IOType _ioType = IOType.None;

        public string Text { get => Text_IOName.Text; set => Text_IOName.Text = value; }
        public string Value { get => Text_IOValue.Text; set => UpdateValue(value); }
        public IOType Type { get => _ioType; set => SetIOType(value); }

        public UIFunctionIO(IOType type)
        {
            InitializeComponent();

            Text = "";
            Value = null;
            Type = type;
        }

        public UIFunctionIO() : this(IOType.Neutral)
        {
        }

        private void SetIOType(IOType type)
        {
            ChangeColourForIOType(type);
        }

        private void ChangeColourForIOType(IOType type)
        {
            var colour = GetColourForIOType(type);

            Background = new SolidColorBrush(colour);
        }

        private Color GetColourForIOType(IOType type)
        {
            return type switch
            {
                IOType.Negative => Color.FromRgb(184, 51, 51),
                IOType.Positive => Color.FromRgb(61, 184, 51),
                IOType.Neutral => Color.FromRgb(59, 119, 228),

                _ => Color.FromRgb(59, 119, 228),
            };
        }

        private void UpdateValue(string value)
        {
            if (value != Value)
            {
                Text_IOValue.Visibility = value == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                Text_IOValue.Text = value;
            }
        }
    }
}
