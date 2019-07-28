using System;
using System.Windows;

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
    }
}
