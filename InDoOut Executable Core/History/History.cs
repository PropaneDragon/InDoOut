using InDoOut_Core.Instancing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InDoOut_Executable_Core.History
{
    public class History : Singleton<History>
    {
        private readonly List<HistoryProperty> _undoStack = new List<HistoryProperty>();
        private readonly List<HistoryProperty> _redoStack = new List<HistoryProperty>();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public ReadOnlyCollection<HistoryProperty> UndoHistory => _undoStack?.AsReadOnly();
        public ReadOnlyCollection<HistoryProperty> RedoHistory => _redoStack?.AsReadOnly();

        public bool AddHistory(string name, Action creationAction, Action undoAction, bool runCreationAction = true)
        {
            var historyProperty = new HistoryProperty(name, creationAction, undoAction, runCreationAction);

            _redoStack.Clear();
            _undoStack.Add(historyProperty);

            return true;
        }

        public bool Undo()
        {
            if (CanUndo)
            {
                var undoProperty = _undoStack.LastOrDefault();
                if (undoProperty != null && undoProperty.Undo())
                {
                    _ = _undoStack.Remove(undoProperty);
                    _redoStack.Add(undoProperty);

                    return true;
                }
            }

            return false;
        }

        public bool Redo()
        {
            if (CanRedo)
            {
                var redoProperty = _redoStack.LastOrDefault();
                if (redoProperty != null && redoProperty.Redo())
                {
                    _ = _redoStack.Remove(redoProperty);
                    _undoStack.Add(redoProperty);

                    return true;
                }
            }

            return false;
        }
    }
}
