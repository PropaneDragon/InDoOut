using InDoOut_Desktop.Display.Selection;
using InDoOut_Desktop.UI.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Desktop.Actions
{
    internal class BlockViewRestingAction : Action
    {
        private readonly IBlockView _blockView = null;

        public BlockViewRestingAction(IBlockView blockView)
        {
            _blockView = blockView;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            _ = _blockView.SelectionManager.Add(selectable);
                        }
                        else
                        {
                            _ = _blockView.SelectionManager.Set(selectable);
                        }
                    }
                    else
                    {
                        _blockView.SelectionManager.Clear();
                    }
                }
            }

            return false;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                var elementsSelected = _blockView.SelectionManager.Selection;

                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<TextBox>(elementsUnderMouse) is TextBox)
                    {
                        return false;
                    }
                    else if (_blockView.GetFirstElementOfType<IUIOutput>(elementsUnderMouse) is IUIOutput output)
                    {
                        Finish(new IOWireDragAction(output, _blockView));

                        return true;
                    }
                    else if (_blockView.GetFirstElementOfType<IUIResult>(elementsUnderMouse) is IUIResult result)
                    {
                        Finish(new VariableWireDragAction(result, _blockView));

                        return true;
                    }
                    else if (_blockView.GetFirstElementOfType<IDraggable>(elementsUnderMouse) != null && elementsSelected.All(element => element is IDraggable))
                    {
                        var draggables = elementsSelected.Cast<IDraggable>();
                        if (draggables.All(draggable => draggable.CanDrag(_blockView)))
                        {
                            Finish(new DraggableDragAction(_blockView, draggables, mousePosition));
                            return true;
                        }

                        return false;
                    }
                    else if (_blockView.GetFirstElementOfType<IScrollable>(elementsUnderMouse) is IScrollable scrollable)
                    {
                        Finish(new ScrollableDragAction(scrollable, mousePosition));

                        return true;
                    }
                }
            }

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            return base.MouseLeftUp(mousePosition);
        }

        public override bool MouseRightUp(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<IUIConnection>(elementsUnderMouse) is IUIConnection connection)
                    {
                        Finish(new ConnectionMenuAction(connection, _blockView, mousePosition));
                    }
                }
            }

            return false;
        }
    }
}
