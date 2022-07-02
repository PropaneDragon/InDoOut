using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_UI_Common.InterfaceElements;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public class CommonProgramDisplayRestingAction : CommonDisplayRestingAction
    {
        public ICommonProgramDisplay ProgramDisplay => Display as ICommonProgramDisplay;

        public CommonProgramDisplayRestingAction(ICommonProgramDisplay display, Feature features = Feature.All) : base(display, features)
        {
        }

        public override bool MouseLeftDown(Point mousePosition) => base.MouseLeftDown(mousePosition);

        public override bool MouseLeftMove(Point mousePosition) => base.MouseLeftMove(mousePosition);

        public override bool MouseLeftUp(Point mousePosition) => base.MouseLeftUp(mousePosition);

        public override bool MouseWheel(int delta) => base.MouseWheel(delta);

        public override bool KeyUp(Key key) => base.KeyUp(key);

        public override bool Drop(Point mousePosition, IDataObject data)
        {
            var formats = data.GetFormats().ToList();
            if (formats.Contains("Function") && data.GetData("Function") is IFunction dataFunction)
            {
                var functionType = dataFunction.GetType();
                var functionBuilder = new InstanceBuilder<IFunction>();
                var function = functionBuilder.BuildInstance(functionType);

                if (function != null)
                {
                    if (mousePosition.X >= 0 && mousePosition.Y >= 0)
                    {
                        function.Metadata["x"] = mousePosition.X.ToString();
                        function.Metadata["y"] = mousePosition.Y.ToString();
                    }

                    var uiFunction = ProgramDisplay?.FunctionCreator?.Create(function);
                    if (uiFunction != null)
                    {
                        return true;
                    }
                    else
                    {
                        Log.Instance.Error("UI Function for ", function, " couldn't be created on the interface");
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function doesn't appear to be able to be placed in the current program.");
                    }
                }
                else
                {
                    Log.Instance.Error("Couldn't build a function for ", functionType, " to place on the interface");
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function couldn't be created and can't be placed in the current program.");
                }
            }
            /*else if (formats.Contains("FileNameW"))
            {
                if (e.Data.GetData("FileNameW") is string[] fileName)
                {
                    var program = await CommonProgramSaveLoad.Instance.LoadProgramDialogAsync(fileName.FirstOrDefault());
                    if (program != null)
                    {
                        _ = ProgramHolder.Instance.RemoveProgram(AssociatedProgram);
                        AssociatedProgram = program;
                    }
                }
            }*/

            return false;
        }
    }
}
