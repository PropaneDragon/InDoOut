using InDoOut_UI_Common.Actions.Copying;
using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public class CommonProgramDisplayRestingAction : Action
    {
        public ICommonProgramDisplay Display { get; set; } = null;

        public CommonProgramDisplayRestingAction(ICommonProgramDisplay display)
        {
            Display = display;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            return false;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            if (Display != null)
            {
                var elementsUnderMouse = Display.GetElementsUnderMouse();
                var selectionManager = Display.SelectionManager;
                var elementsSelected = selectionManager.Selection;

                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0 && Display.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(Display) && !selectionManager.Contains(selectable))
                {
                    _ = selectionManager.Set(selectable);
                }

                elementsSelected = selectionManager.Selection;

                if (elementsUnderMouse.Count > 0)
                {
                    if (Display.GetFirstElementOfType<TextBox>(elementsUnderMouse) is TextBox)
                    {
                        return false;
                    }
                    else if (Display.GetFirstElementOfType<IUIConnectionStart>(elementsUnderMouse) is IUIConnectionStart start)
                    {
                        Finish(new CommonWireDragAction(start, Display));

                        return true;
                    }
                    else if (Display.GetFirstElementOfType<IDraggable>(elementsUnderMouse) != null && elementsSelected.Any(element => element is IDraggable draggable && draggable.CanDrag(Display)))
                    {
                        var draggables = elementsSelected.Where(element => element is IDraggable draggable && draggable.CanDrag(Display)).Cast<IDraggable>();

                        Finish(new DraggableDragAction(Display, draggables, mousePosition));

                        return true;
                    }
                    else if (Display.GetFirstElementOfType<IScrollable>(elementsUnderMouse) is IScrollable scrollable)
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
            if (Display != null)
            {
                var elementsUnderMouse = Display.GetElementsUnderMouse();
                elementsUnderMouse.Reverse();

                if (elementsUnderMouse.Count > 0)
                {
                    if (Display.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(Display))
                    {
                        _ = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ? Display.SelectionManager.Add(selectable, true) : Display.SelectionManager.Set(selectable, false);
                    }
                    else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        Display.SelectionManager.Clear();
                    }

                    return true;
                }
            }

            return false;
        }

        public override bool KeyUp(Key key)
        {
            if (Display != null)
            {
                var elementsSelected = Display.SelectionManager.Selection;

                if (key == Key.D && Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && elementsSelected.All(element => element is ICopyable copyable && copyable.CanCopy(Display)))
                {
                    var copyables = elementsSelected.Cast<ICopyable>();

                    foreach (var copyable in copyables)
                    {
                        var copy = copyable.CreateCopy(Display);
                        if (copy != null)
                        {
                            return copyable.CopyTo(copy);
                        }
                    }
                }
                else if (key == Key.Delete && !(Keyboard.FocusedElement is TextBox) && elementsSelected.All(element => element is IDeletable deletable && deletable.CanDelete(Display)))
                {
                    var deletables = elementsSelected.Cast<IDeletable>();

                    foreach (var deletable in deletables)
                    {
                        _ = Display?.DeletableRemover?.Remove(deletable);
                    }
                }
            }

            return false;
        }
    }
}
