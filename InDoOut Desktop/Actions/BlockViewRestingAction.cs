using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Windows;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Desktop.Actions
{
    internal class BlockViewRestingAction : Action
    {
        private readonly IBlockView _blockView = null;
        private readonly ActionHandler _commonDisplayActions;

        public BlockViewRestingAction(IBlockView blockView)
        {
            _blockView = blockView;
            _commonDisplayActions = new ActionHandler(new CommonProgramDisplayRestingAction(_blockView));
        }

        public override bool MouseRightUp(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<IUIConnection>(elementsUnderMouse) is IUIConnection connection)
                    {
                        Finish(new ConnectionMenuAction(connection, _blockView));

                        return true;
                    }
                }
            }

            return _commonDisplayActions?.MouseRightUp(mousePosition) ?? false;
        }

        public override bool MouseDoubleClick(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<IUIFunction>(elementsUnderMouse) is IUIFunction uiFunction && uiFunction?.AssociatedFunction is ISelfRunnerFunction selfRunnerFunction && selfRunnerFunction.LoadedProgram != null)
                    {
                        var previewWindow = new PopUpBlockViewWindow(selfRunnerFunction.LoadedProgram)
                        {
                            Width = _blockView.ViewSize.Width - 150,
                            Height = _blockView.ViewSize.Height - 150
                        };

                        if (_blockView is DependencyObject dependencyObject)
                        {
                            previewWindow.Owner = Window.GetWindow(dependencyObject);
                        }

                        previewWindow.Show();
                        _ = previewWindow.Activate();

                        return true;
                    }
                }
            }

            return _commonDisplayActions?.MouseDoubleClick(mousePosition) ?? false;
        }

        public override bool MouseRightMove(Point mousePosition) => _commonDisplayActions?.MouseRightMove(mousePosition) ?? false;

        public override bool MouseRightDown(Point mousePosition) => _commonDisplayActions?.MouseRightDown(mousePosition) ?? false;

        public override bool MouseNoMove(Point mousePosition) => _commonDisplayActions?.MouseNoMove(mousePosition) ?? false;

        public override bool MouseLeftDown(Point mousePosition) => _commonDisplayActions?.MouseLeftDown(mousePosition) ?? false;

        public override bool MouseLeftMove(Point mousePosition) => _commonDisplayActions?.MouseLeftMove(mousePosition) ?? false;

        public override bool MouseLeftUp(Point mousePosition) => _commonDisplayActions?.MouseLeftUp(mousePosition) ?? false;

        public override bool MouseWheel(int delta) => _commonDisplayActions?.MouseWheel(delta) ?? false;

        public override bool KeyUp(Key key) => _commonDisplayActions?.KeyUp(key) ?? false;

        public override bool KeyDown(Key key) => _commonDisplayActions?.KeyDown(key) ?? false;
    }
}
