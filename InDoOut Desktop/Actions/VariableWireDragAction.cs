using InDoOut_Core.Variables;
using InDoOut_Desktop.UI.Interfaces;
using System;

namespace InDoOut_Desktop.Actions
{
    internal class VariableWireDragAction : AbstractWireDragAction
    {
        private IVariableStore _variableStore = null;

        public VariableWireDragAction(IUIConnectionStart start, IBlockView view, IVariableStore variableStore) : base(start, view)
        {
            _variableStore = variableStore;
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

                if (property != null && result != null && _variableStore != null)
                {
                    var variableName = result.VariableName;

                    if (string.IsNullOrEmpty(variableName))
                    {
                        variableName = Guid.NewGuid().ToString();

                        result.VariableName = variableName;
                    }

                    if (!_variableStore.VariableExists(variableName))
                    {
                        _ = _variableStore.SetVariable(variableName, "");
                    }

                    var variable = _variableStore.GetVariable(variableName);

                    property.AssociatedVariable = variable;

                    return variable != null;
                }
            }

            return false;
        }
    }
}
