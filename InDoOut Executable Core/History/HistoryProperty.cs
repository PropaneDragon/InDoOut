using System;

namespace InDoOut_Executable_Core.History
{
    public class HistoryProperty
    {
        private enum LastAction { Undo, Redo };

        private readonly Action _creationAction = null;
        private readonly Action _undoAction = null;

        private LastAction _lastAction = LastAction.Redo;

        public string Name { get; private set; } = null;

        public HistoryProperty(string name, Action creationAction, Action undoAction, bool runCreationAction = true)
        {
            Name = name;

            _creationAction = creationAction;
            _undoAction = undoAction;

            if (runCreationAction)
            {
                creationAction?.Invoke();
            }
        }

        public bool Undo()
        {
            if (_lastAction != LastAction.Undo)
            {
                _lastAction = LastAction.Undo;
                _undoAction?.Invoke();

                return true;
            }

            return false;
        }

        public bool Redo()
        {
            if (_lastAction != LastAction.Redo)
            {
                _lastAction = LastAction.Redo;
                _creationAction?.Invoke();

                return true;
            }

            return false;
        }
    }
}
