using InDoOut_UI_Common.Actions.Copying;
using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public class CommonDisplayRestingAction : Action
    {
        public ICommonDisplay Display { get; protected set; } = null;
        public Feature Features { get; set; } = Feature.All;

        [Flags]
        public enum Feature
        {
            None = 1 << 0,
            WireDragging = 1 << 1,
            Scrolling = 1 << 2,
            Selection = 1 << 3,
            Copying = 1 << 4,
            Deletion = 1 << 5,
            Dragging = 1 << 6,
            All = WireDragging | Scrolling | Selection | Copying | Deletion | Dragging
        }

        private CommonDisplayRestingAction()
        {
        }

        public CommonDisplayRestingAction(ICommonDisplay display, Feature features = Feature.All) : this()
        {
            Display = display;
            Features = features;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (Display != null)
            {
                var elementsUnderMouse = Display.GetElementsUnderMouse();
                var selectionManager = Display.SelectionManager;

                elementsUnderMouse.Reverse();

                if (Features.HasFlag(Feature.Selection) && elementsUnderMouse.Count > 0 && Display.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(Display) && selectionManager != null && !selectionManager.Contains(selectable))
                {
                    _ = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ? Display.SelectionManager.Add(selectable, true) : Display.SelectionManager.Set(selectable, false);
                }

                if (elementsUnderMouse.Count > 0)
                {
                    if (Display.GetFirstElementOfType<TextBox>(elementsUnderMouse) is TextBox)
                    {
                        return false;
                    }
                }
            }

            return base.MouseLeftDown(mousePosition);
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            var elementsUnderMouse = Display.GetElementsUnderMouse();
            var selectionManager = Display.SelectionManager;
            var elementsSelected = selectionManager?.Selection;

            elementsUnderMouse.Reverse();

            if (elementsUnderMouse.Count > 0)
            {
                if (Display.GetFirstElementOfType<TextBox>(elementsUnderMouse) is TextBox)
                {
                    return false;
                }
                else if (Features.HasFlag(Feature.WireDragging) && Display.GetFirstElementOfType<IUIConnectionStart>(elementsUnderMouse) is IUIConnectionStart start)
                {
                    Finish(new CommonWireDragAction(start, Display));

                    return true;
                }
                else if (Features.HasFlag(Feature.Dragging) && Display.GetFirstElementOfType<IDraggable>(elementsUnderMouse) != null && elementsSelected != null && elementsSelected.Any(element => element is IDraggable draggable && draggable.CanDrag(Display)))
                {
                    var draggables = elementsSelected.Where(element => element is IDraggable draggable && draggable.CanDrag(Display)).Cast<IDraggable>();

                    Finish(new DraggableDragAction(Display, draggables, mousePosition));

                    return true;
                }
                else if (Features.HasFlag(Feature.Scrolling) && Display.GetFirstElementOfType<IScrollable>(elementsUnderMouse) is IScrollable scrollable)
                {
                    Finish(new ScrollableDragAction(scrollable, mousePosition));

                    return true;
                }
            }

            return base.MouseLeftMove(mousePosition);
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            if (Display != null)
            {
                var elementsUnderMouse = Display.GetElementsUnderMouse();
                elementsUnderMouse.Reverse();

                if (Features.HasFlag(Feature.Selection) && elementsUnderMouse.Count > 0 && Display.GetFirstElementOfType<ISelectable>(elementsUnderMouse) == null)
                {
                    if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        Display.SelectionManager?.Clear();
                    }

                    return true;
                }
            }

            return base.MouseLeftUp(mousePosition);
        }

        public override bool KeyUp(Key key)
        {
            if (Display != null)
            {
                var elementsSelected = Display.SelectionManager?.Selection;

                if (Features.HasFlag(Feature.Copying) && key == Key.D && Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && elementsSelected != null && elementsSelected.All(element => element is ICopyable copyable && copyable.CanCopy(Display)))
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
                else if (Features.HasFlag(Feature.Deletion) && key == Key.Delete && !(Keyboard.FocusedElement is TextBox) && elementsSelected != null && elementsSelected.All(element => element is IDeletable deletable && deletable.CanDelete(Display)))
                {
                    var deletables = elementsSelected.Cast<IDeletable>();

                    foreach (var deletable in deletables)
                    {
                        _ = Display?.DeletableRemover?.Remove(deletable);
                    }
                }
            }

            return base.KeyUp(key);
        }

        public override bool MouseWheel(int delta) => base.MouseWheel(delta);
    }
}
