using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementHost : UserControl, IResizable
    {
        public DisplayElementHost()
        {
            InitializeComponent();
        }

        public bool CanResize(IScreen screen)
        {
            return true;
        }

        public void ResizeEnded(IScreen screen)
        {
        }

        public void ResizeStarted(IScreen screen)
        {
        }
    }
}
