using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public class CommonWireDragAction : AbstractWireDragAction
    {
        private IUIConnectionStart _start = null;

        public CommonWireDragAction(IUIConnectionStart start, ICommonDisplay display) : base(start, display)
        {
            _start = start;
        }

        protected override bool ViableEnd(IUIConnectionEnd endConnection)
        {
            if (_start is IUIOutput)
            {
                return endConnection is IUIInput;
            }
            else if(_start is IUIResult)
            {
                return endConnection is IUIProperty;
            }

            return false;
        }

        protected override bool FinishConnection(IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (start is IUIOutput uiOutput && end is IUIInput uiInput)
            {
                var input = uiInput.AssociatedInput;
                var output = uiOutput.AssociatedOutput;

                return (input?.CanAcceptConnection(output) ?? false) && (output?.Connect(input) ?? false);
            }
            else if (start is IUIResult uiResult && end is IUIProperty uiProperty)
            {
                var property = uiProperty.AssociatedProperty;
                var result = uiResult.AssociatedResult;

                return (property?.CanAcceptConnection(result) ?? false) && (result?.Connect(property) ?? false);
            }

            return false;
        }
    }
}
