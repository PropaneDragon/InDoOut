using System.Collections.Generic;
using System.Linq;
using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions
{
    internal class IOWireDragAction : AbstractWireDragAction
    {
        public IOWireDragAction(IUIConnectionStart start, IBlockView view) : base(start, view)
        {
        }

        protected override bool ViableEnd(IUIConnectionEnd endConnection)
        {
            return endConnection is IUIInput;
        }

        protected override bool FinishConnection(IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (start is IUIOutput uiOutput && end is IUIInput uiInput)
            {
                var input = uiInput.AssociatedInput;
                var output = uiOutput.AssociatedOutput;

                return (input?.CanAcceptConnection(output) ?? false) && (output?.Connect(input) ?? false);
            }

            return false;
        }
    }
}
