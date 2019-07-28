using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InDoOut_Desktop.Actions
{
    internal class BlockViewRestingAction : Action
    {
        private ScrollViewer _scrollViewer = null;

        public BlockViewRestingAction(ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (_scrollViewer != null)
            {
                var hitResult = VisualTreeHelper.HitTest(_scrollViewer, mousePosition);
                if (hitResult != null && hitResult.VisualHit != null)
                {
                    if (hitResult.VisualHit is ScrollViewer scrollViewer)
                    {
                        Finish(new ScrollViewDragAction(scrollViewer, mousePosition));

                        return true;
                    }
                    else if (FindParentOrChild<UserControl>(hitResult.VisualHit) is IDraggable draggable)
                    {
                        if (draggable.CanDrag())
                        {
                            Finish(new DraggableDragAction(draggable, mousePosition));
                            return true;
                        }

                        return false;
                    }
                    else if(hitResult.VisualHit is UIElement element)
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
