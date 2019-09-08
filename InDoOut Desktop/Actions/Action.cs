using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Desktop.Actions
{
    internal abstract class Action : IAction
    {
        public event EventHandler<ActionCompleteEventArgs> ActionComplete;

        public virtual bool MouseNoMove(Point mousePosition) => false;
        public virtual bool MouseLeftDown(Point mousePosition) => false;
        public virtual bool MouseLeftMove(Point mousePosition) => false;
        public virtual bool MouseLeftUp(Point mousePosition) => false;
        public virtual bool MouseRightDown(Point mousePosition) => false;
        public virtual bool MouseRightMove(Point mousePosition) => false;
        public virtual bool MouseRightUp(Point mousePosition) => false;
        public virtual bool KeyDown(Key key) => false;
        public virtual bool KeyUp(Key key) => false;

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
