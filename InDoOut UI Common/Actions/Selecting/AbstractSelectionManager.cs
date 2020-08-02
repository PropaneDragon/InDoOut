using System.Collections.Generic;

namespace InDoOut_UI_Common.Actions.Selecting
{
    public abstract class AbstractSelectionManager<SelectableType> : ISelectionManager<SelectableType> where SelectableType : ISelectable
    {
        private readonly List<SelectableType> _selection = new List<SelectableType>();

        public List<SelectableType> Selection => new List<SelectableType>(_selection);

        public AbstractSelectionManager()
        {
        }

        public void Clear()
        {
            foreach (var item in Selection)
            {
                _ = Remove(item);
            }
        }

        public bool Add(SelectableType selectable, bool toggleIfAlreadyInserted = false)
        {
            if (selectable != null)
            {
                if (!Contains(selectable))
                {
                    _selection.Add(selectable);

                    NotifySelectableStarted(selectable);

                    return true;
                }
                else if (toggleIfAlreadyInserted)
                {
                    return Remove(selectable);
                }
            }

            return false;
        }

        public bool Set(SelectableType selectable, bool toggleIfAlreadyInserted = false)
        {
            if (selectable != null)
            {
                var stayCleared = Contains(selectable) && toggleIfAlreadyInserted;

                Clear();

                return stayCleared || Add(selectable);
            }

            return false;
        }

        public bool Remove(SelectableType selectable)
        {
            if (selectable != null && Contains(selectable))
            {
                NotifySelectableEnded(selectable);

                _ = _selection.Remove(selectable);

                return true;
            }

            return false;
        }

        public bool Contains(SelectableType selectable)
        {
            return selectable != null && _selection.Contains(selectable);
        }

        protected abstract void NotifySelectableStarted(SelectableType selectable);
        protected abstract void NotifySelectableEnded(SelectableType selectable);
    }
}
