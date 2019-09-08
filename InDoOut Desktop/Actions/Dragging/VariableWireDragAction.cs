using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions.Dragging
{
    internal class VariableWireDragAction : AbstractWireDragAction
    {
        public VariableWireDragAction(IUIConnectionStart start, IBlockView view) : base(start, view)
        {
        }

        protected override bool ViableEnd(IUIConnectionEnd endConnection)
        {
            return endConnection is IUIProperty;
        }

        protected override bool FinishConnection(IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (start is IUIResult uiResult && end is IUIProperty uiProperty)
            {
                var property = uiProperty.AssociatedProperty;
                var result = uiResult.AssociatedResult;

                return (property?.CanAcceptConnection(result) ?? false) && (result?.Connect(property) ?? false);
            }

            return false;
        }
    }
}
