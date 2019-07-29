using System;
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

        private IOType _ioType = IOType.None;

        public string Text { get => Text_IOName.Text; set => Text_IOName.Text = value; }
        public IOType Type { get => _ioType; set => SetIOType(value); }

        public UIFunctionIO(IOType type)
        {
            InitializeComponent();

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
            switch (type)
            {
                case IOType.Negative:
                    return Color.FromRgb(184, 51, 51);
                case IOType.Positive:
                    return Color.FromRgb(61, 184, 51);
                case IOType.Neutral:
                    return Color.FromRgb(59, 119, 228);
            }

            return Color.FromRgb(59, 119, 228);
;        }
    }
}
