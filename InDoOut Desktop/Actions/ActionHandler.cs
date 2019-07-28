using System;
using System.Windows;

namespace InDoOut_Desktop.Actions
{
    internal class ActionHandler : IActionHandler
    {
        private IAction _currentAction = null;

        public IAction DefaultAction { get; set; } = null;
        public IAction CurrentAction { get => _currentAction; set => SetAction(value); }

        public event EventHandler<ActionCompleteEventArgs> ActionComplete;

        public ActionHandler(IAction defaultAction = null)
        {
            DefaultAction = defaultAction;
            CurrentAction = defaultAction;
        }

        public bool MouseLeftDown(Point mousePosition)
        {
            return CurrentAction?.MouseLeftDown(mousePosition) ?? false;
        }

        public bool MouseLeftMove(Point mousePosition)
        {
            return CurrentAction?.MouseLeftMove(mousePosition) ?? false;
        }

        public bool MouseLeftUp(Point mousePosition)
        {
            return CurrentAction?.MouseLeftUp(mousePosition) ?? false;
        }

        public bool MouseRightDown(Point mousePosition)
        {
            return CurrentAction?.MouseRightDown(mousePosition) ?? false;
        }

        public bool MouseRightMove(Point mousePosition)
        {
            return CurrentAction?.MouseRightMove(mousePosition) ?? false;
        }

        public bool MouseRightUp(Point mousePosition)
        {
            return CurrentAction?.MouseRightUp(mousePosition) ?? false;
        }

        private void SetAction(IAction action)
        {
            if (_currentAction != null)
            {
                _currentAction.ActionComplete -= Action_ActionComplete;
            }

            _currentAction = action ?? DefaultAction;

            if (_currentAction != null)
            {
                _currentAction.ActionComplete += Action_ActionComplete;
            }
        }

        private void Action_ActionComplete(object sender, ActionCompleteEventArgs e)
        {
            ActionComplete?.Invoke(this, e);

            SetAction(e.NextAction);
        }
    }
}
