using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

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
                    else if (_blockView.GetFirstElementOfType<IDraggable>(elementsUnderMouse) is IDraggable draggable)
                    {
                        if (draggable.CanDrag())
                        {
                            Finish(new DraggableDragAction(_blockView, draggable, mousePosition));
                            return true;
                        }

                        return false;
                    }
                    else if (_blockView.GetFirstElementOfType<IScrollable>(elementsUnderMouse) is IScrollable scrollable)
                    {
                        Finish(new ScrollableDragAction(scrollable, mousePosition));

                        return true;
                    }

                    /*else if(hitVisual is UIElement element)
                    {
                        Finish(new ElementDragAction(element, mousePosition));

                        return true;
                    }*/
                }
            }

            return false;
        }
    }
}
