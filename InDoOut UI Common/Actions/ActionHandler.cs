using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public class ActionHandler : IActionHandler
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

        public bool MouseNoMove(Point mousePosition) => CurrentAction?.MouseNoMove(mousePosition) ?? false;
        public bool MouseLeftDown(Point mousePosition) => CurrentAction?.MouseLeftDown(mousePosition) ?? false;
        public bool MouseLeftMove(Point mousePosition) => CurrentAction?.MouseLeftMove(mousePosition) ?? false;
        public bool MouseLeftUp(Point mousePosition) => CurrentAction?.MouseLeftUp(mousePosition) ?? false;
        public bool MouseRightDown(Point mousePosition) => CurrentAction?.MouseRightDown(mousePosition) ?? false;
        public bool MouseRightMove(Point mousePosition) => CurrentAction?.MouseRightMove(mousePosition) ?? false;
        public bool MouseRightUp(Point mousePosition) => CurrentAction?.MouseRightUp(mousePosition) ?? false;
        public bool MouseDoubleClick(Point mousePosition) => CurrentAction?.MouseDoubleClick(mousePosition) ?? false;
        public bool MouseWheel(int delta) => CurrentAction?.MouseWheel(delta) ?? false;
        public bool KeyDown(Key key) => CurrentAction?.KeyDown(key) ?? false;
        public bool KeyUp(Key key) => CurrentAction?.KeyUp(key) ?? false;

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
