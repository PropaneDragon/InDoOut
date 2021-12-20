using System;

namespace InDoOut_UI_Common.Extensions.Window
{
    public static class WindowResizeExtensions
    {
        [Flags]
        public enum WindowResizeConstraint
        {
            Width = 1,
            Height = 2
        }

        public static void ResizeToOwner(this System.Windows.Window window, WindowResizeConstraint constraint = WindowResizeConstraint.Height)
        {
            if (window.Owner != null)
            {
                if (constraint.HasFlag(WindowResizeConstraint.Height))
                {
                    window.Height = window.Owner.Height - 100;
                    window.Top = window.Owner.Top + 50;
                }

                if (constraint.HasFlag(WindowResizeConstraint.Width))
                {
                    window.Width = window.Owner.Width - 100;
                    window.Left = window.Owner.Left + 50;
                }
            }
        }
    }
}
