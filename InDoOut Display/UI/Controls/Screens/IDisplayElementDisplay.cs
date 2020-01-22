using InDoOut_Display_Core.Elements;

namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IDisplayElementDisplay
    {
        bool AddDisplayElement(IDisplayElement displayElement);
        bool RemoveDisplayElement(IDisplayElement displayElement);
    }
}
