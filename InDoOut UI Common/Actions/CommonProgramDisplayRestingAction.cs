using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Actions
{
    public class CommonProgramDisplayRestingAction : CommonDisplayRestingAction
    {
        private readonly DispatcherTimer _draggedElementDeleter = new(DispatcherPriority.Normal);

        private bool _stillDragging = false;
        private IUIFunction _ghostFunction = null;
        private Point _lastGhostFunctionPosition = new();

        public ICommonProgramDisplay ProgramDisplay => Display as ICommonProgramDisplay;

        public CommonProgramDisplayRestingAction(ICommonProgramDisplay display, Feature features = Feature.All) : base(display, features)
        {
            _draggedElementDeleter.Tick += DraggedElementDeleter_Tick;
        }

        public override bool MouseLeftDown(Point mousePosition) => base.MouseLeftDown(mousePosition);

        public override bool MouseLeftMove(Point mousePosition) => base.MouseLeftMove(mousePosition);

        public override bool MouseLeftUp(Point mousePosition) => base.MouseLeftUp(mousePosition);

        public override bool MouseWheel(int delta) => base.MouseWheel(delta);

        public override bool KeyUp(Key key) => base.KeyUp(key);

        public override bool DragEnter(Point mousePosition, IDataObject data)
        {
            _stillDragging = true;

            if (_ghostFunction != null)
            {
                return DragOver(mousePosition, data);
            }
            else
            {
                var formats = data.GetFormats().ToList();
                if (formats.Contains("Function"))
                {
                    _ghostFunction = CreateGhostFunctionFromDropData(mousePosition, data);
                    _lastGhostFunctionPosition = mousePosition;

                    return _ghostFunction != null;
                }
            }

            return false;
        }

        public override bool DragOver(Point mousePosition, IDataObject data)
        {
            _stillDragging = true;
            _draggedElementDeleter.Stop();

            if (_ghostFunction != null && _ghostFunction is FrameworkElement frameworkElementFunction)
            {
                _lastGhostFunctionPosition = OffsetPositionByFunctionTitle(mousePosition, _ghostFunction);

                ProgramDisplay?.SetPosition(frameworkElementFunction, _lastGhostFunctionPosition);

                return true;
            }

            return false;
        }

        public override bool DragLeave(Point mousePosition, IDataObject data)
        {
            _stillDragging = false;

            _draggedElementDeleter.Interval = TimeSpan.FromMilliseconds(2);
            _draggedElementDeleter.Start();

            return false;
        }

        public override bool Drop(Point mousePosition, IDataObject data)
        {
            var formats = data.GetFormats().ToList();
            if (formats.Contains("Function"))
            {
                var finalPosition = _lastGhostFunctionPosition.X > 0 && _lastGhostFunctionPosition.Y > 0 ? _lastGhostFunctionPosition : mousePosition;

                _ = CreateFunctionFromDropData(finalPosition, data);
            }
            
            if (formats.Contains("FileNameW"))
            {
                if (data.GetData("FileNameW") is string[] fileName)
                {
                    var program = Task.Run(async () => await CommonProgramSaveLoad.Instance.LoadProgramAsync(fileName.FirstOrDefault())).Result;
                    if (program != null)
                    {
                        ProgramDisplay.AssociatedProgram = program;
                    }
                }
            }

            _stillDragging = false;

            DeleteGhostFunction();

            return false;
        }

        private IUIFunction CreateGhostFunctionFromDropData(Point initialPosition, IDataObject data)
        {
            var function = CreateFunctionFromDropData(initialPosition, data);
            if (function != null && function is FrameworkElement newFrameworkElementFunction)
            {
                newFrameworkElementFunction.Opacity = 0.5;
            }

            return function;
        }

        private IUIFunction CreateFunctionFromDropData(Point iniitialPosition, IDataObject data)
        {
            var formats = data.GetFormats().ToList();
            if (formats.Contains("Function") && data.GetData("Function") is IFunction dataFunction)
            {
                var functionType = dataFunction.GetType();
                var functionBuilder = new InstanceBuilder<IFunction>();
                var function = functionBuilder.BuildInstance(functionType);

                if (function != null)
                {
                    if (iniitialPosition.X >= 0 && iniitialPosition.Y >= 0)
                    {
                        function.Metadata["x"] = iniitialPosition.X.ToString();
                        function.Metadata["y"] = iniitialPosition.Y.ToString();
                    }

                    var uiFunction = ProgramDisplay?.FunctionCreator?.Create(function);
                    if (uiFunction == null)
                    {
                        Log.Instance.Error("UI Function for ", function, " couldn't be created on the interface");
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function doesn't appear to be able to be placed in the current program.");
                    }

                    return uiFunction;
                }
                else
                {
                    Log.Instance.Error("Couldn't build a function for ", functionType, " to place on the interface");
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function couldn't be created and can't be placed in the current program.");
                }
            }

            return null;
        }

        private Point OffsetPositionByFunctionTitle(Point originalPosition, IUIFunction function)
        {
            var offsetPosition = originalPosition;

            if (function != null && function is FrameworkElement frameworkElementFunction)
            {
                offsetPosition.Offset(-frameworkElementFunction.ActualWidth / 2d, -function.GetTitleOffset().Y);
            }

            return offsetPosition;
        }

        private void DeleteGhostFunction()
        {
            if (_ghostFunction != null && _ghostFunction is FrameworkElement oldFrameworkElementFunction)
            {
                ProgramDisplay?.Remove(oldFrameworkElementFunction);

                _ghostFunction = null;
            }
        }

        private void DraggedElementDeleter_Tick(object sender, EventArgs e)
        {
            if (!_stillDragging)
            {
                DeleteGhostFunction();
            }

            _draggedElementDeleter.Stop();
        }
    }
}
