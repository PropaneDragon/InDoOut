using InDoOut_Desktop.UI.Controls.CoreEntityRepresentation;
using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InDoOut_Desktop.Actions
{
    internal class BlockViewRestingAction : Action
    {
        private FrameworkElement _topLevel = null;
        private IBlockView _blockView = null;

        public BlockViewRestingAction(FrameworkElement topLevel, IBlockView blockView)
        {
            _topLevel = topLevel;
            _blockView = blockView;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (_topLevel != null && _blockView != null)
            {
                var hitResult = VisualTreeHelper.HitTest(_topLevel, mousePosition);
                if (hitResult != null && hitResult.VisualHit != null)
                {
                    var hitVisual = hitResult.VisualHit;
                    if (hitVisual is ScrollViewer scrollViewer)
                    {
                        Finish(new ScrollViewDragAction(scrollViewer, mousePosition));

                        return true;
                    }
                    else if (FindParentOrChild<UIOutput>(hitVisual) is UIOutput output)
                    {
                        Finish(new WireDragAction(output, _blockView));

                        return true;
                    }
                    else if (FindParentOrChild<UserControl>(hitVisual) is IDraggable draggable)
                    {
                        if (draggable.CanDrag())
                        {
                            Finish(new DraggableDragAction(draggable, mousePosition));
                            return true;
                        }

                        return false;
                    }
                    else if(hitVisual is UIElement element)
                    {
                        Finish(new ElementDragAction(element, mousePosition));

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
