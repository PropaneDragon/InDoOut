using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display_Core.Screens
{
    /// <summary>
    /// Represents a screen that hosts <see cref="IProgram"/>s, as well as an internal <see cref="IScreen"/>
    /// that hosts UI elements inside it connectable from this screen.
    /// </summary>
    public interface IScreenConnections : ICommonProgramDisplay
    {
        /// <summary>
        /// The internal <see cref="IScreen"/> that hosts other elements.
        /// </summary>
        IScreen CurrentScreen { get; }
    }
}
