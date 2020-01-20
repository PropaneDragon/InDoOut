using InDoOut_Core.Entities.Functions;

namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IFunctionHost
    {
        public bool AddFunction(IFunction function);
        public bool RemoveFunction(IFunction function);
    }
}
