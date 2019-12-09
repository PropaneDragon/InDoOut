using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions.Copying;
using InDoOut_Desktop.Actions.Deleting;
using InDoOut_Desktop.Actions.Dragging;
using InDoOut_Desktop.Actions.Selecting;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Windows;
using InDoOut_UI_Common.Actions;
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
            return false;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                var selectionManager = _blockView.SelectionManager;
                var elementsSelected = selectionManager.Selection;

                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0 && _blockView.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(_blockView) && !selectionManager.Contains(selectable))
                {
                    _ = selectionManager.Set(selectable);
                }

                elementsSelected = selectionManager.Selection;

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
                    else if (_blockView.GetFirstElementOfType<IDraggable>(elementsUnderMouse) != null && elementsSelected.Any(element => element is IDraggable draggable && draggable.CanDrag(_blockView)))
                    {
                        var draggables = elementsSelected.Where(element => element is IDraggable draggable && draggable.CanDrag(_blockView)).Cast<IDraggable>();

                        Finish(new DraggableDragAction(_blockView, draggables, mousePosition));

                        return true;
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
            if (_blockView != null)
            {
                var elementsUnderMouse = _blockView.GetElementsUnderMouse();
                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0)
                {
                    if (_blockView.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(_blockView))
                    {
                        _ = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ? _blockView.SelectionManager.Add(selectable, true) : _blockView.SelectionManager.Set(selectable, false);
                    }
                    else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        _blockView.SelectionManager.Clear();
                    }

                    return true;
                }
            }

            return false;
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
                        Finish(new ConnectionMenuAction(connection, _blockView, mousePosition));
                    }
                }
            }

            return false;
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
                    }
                }
            }

            return false;
        }

        public override bool KeyUp(Key key)
        {
            if (_blockView != null)
            {
                var elementsSelected = _blockView.SelectionManager.Selection;

                if (key == Key.D && Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && elementsSelected.All(element => element is ICopyable copyable && copyable.CanCopy(_blockView)))
                {
                    var copyables = elementsSelected.Cast<ICopyable>();

                    foreach (var copyable in copyables)
                    {
                        var copy = copyable.CreateCopy(_blockView);
                        if (copy != null)
                        {
                            return copyable.CopyTo(copy);
                        }
                    }
                }
                else if (key == Key.Delete && !(Keyboard.FocusedElement is TextBox) && elementsSelected.All(element => element is IDeletable deletable && deletable.CanDelete(_blockView)))
                {
                    var deletables = elementsSelected.Cast<IDeletable>();

                    foreach (var deletable in deletables)
                    {
                        deletable.Deleted(_blockView);
                    }
                }
            }

            return false;
        }
    }
}
