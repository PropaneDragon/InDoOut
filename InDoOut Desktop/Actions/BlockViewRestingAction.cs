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
                        Finish(new ScrollViewDragAction(_scrollViewer, mousePosition));

                        return true;
                    }
                    else
                    {

                    }
                }
            }

            return false;
        }
    }
}
