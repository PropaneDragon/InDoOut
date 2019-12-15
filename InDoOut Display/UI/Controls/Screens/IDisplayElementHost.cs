using InDoOut_Display_Core.Elements;

namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IDisplayElementHost
    {
        bool AddDisplayElement(IDisplayElement displayElement);
        bool RemoveDisplayElement(IDisplayElement displayElement);
    }
}
