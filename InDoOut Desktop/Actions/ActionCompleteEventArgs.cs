using System;
using System.Collections.Generic;
using System.Text;

namespace InDoOut_Desktop.Actions
{
    internal class ActionCompleteEventArgs : EventArgs
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
