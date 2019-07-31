using System;
using System.Windows;
using System.Windows.Media;

namespace InDoOut_Desktop.Actions
{
    internal abstract class Action : IAction
    {
        public event EventHandler<ActionCompleteEventArgs> ActionComplete;

        public virtual bool MouseLeftDown(Point mousePosition) { return false; }
        public virtual bool MouseLeftMove(Point mousePosition) { return false; }
        public virtual bool MouseLeftUp(Point mousePosition) { return false; }
        public virtual bool MouseRightDown(Point mousePosition) { return false; }
        public virtual bool MouseRightMove(Point mousePosition) { return false; }
        public virtual bool MouseRightUp(Point mousePosition) { return false; }

        protected void Finish(IAction nextAction)
        {
            ActionComplete?.Invoke(this, new ActionCompleteEventArgs(this, nextAction));
        }

        protected void Abort()
        {
            Finish(null);
        }

        protected T FindParentOrChild<T>(DependencyObject child) where T : DependencyObject
        {
            if (child is T dependencyObject)
            {
                return child as T;
            }

            return FindParent<T>(child);
        }

        protected T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child != null)
            {
                var parent = VisualTreeHelper.GetParent(child);
                if (parent == null)
                {
                    return null;
                }
                else if (parent is T control)
                {
                    return control;
                }
                else if (typeof(T).IsAssignableFrom(parent.GetType()))
                {
                    return parent as T;
                }

                return FindParent<T>(parent);
            }

            return default;
        }
    }
}
