using System;

namespace InDoOut_UI_Common.Actions
{
    public class ActionCompleteEventArgs : EventArgs
    {
        public IAction CurrentAction { get; private set; } = null;
        public IAction NextAction { get; private set; } = null;

        public ActionCompleteEventArgs(IAction currentAction, IAction nextAction = null)
        {
            CurrentAction = currentAction;
            NextAction = nextAction;
        }
    }
}
