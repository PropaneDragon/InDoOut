using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;

namespace InDoOut_Desktop.Actions.Selecting
{
    internal class SelectionManager : ISelectionManager
    {
        private readonly IBlockView _associatedBlockView = null;
        private readonly List<ISelectable> _selection = new List<ISelectable>();

        public List<ISelectable> Selection => new List<ISelectable>(_selection);

        public SelectionManager(IBlockView blockView)
        {
            _associatedBlockView = blockView;
        }

        public void Clear()
        {
            foreach (var item in Selection)
            {
                _ = Remove(item);
            }
        }

        public bool Add(ISelectable selectable, bool toggleIfAlreadyInserted = false)
        {
            if (selectable != null)
            {
                if (!Contains(selectable))
                {
                    _selection.Add(selectable);

                    selectable.SelectionStarted(_associatedBlockView);

                    return true;
                }
                else if (toggleIfAlreadyInserted)
                {
                    return Remove(selectable);
                }
            }

            return false;
        }

        public bool Set(ISelectable selectable, bool toggleIfAlreadyInserted = false)
        {
            if (selectable != null)
            {
                var stayCleared = Contains(selectable) && toggleIfAlreadyInserted;

                Clear();

                return !stayCleared ? Add(selectable) : true;
            }

            return false;
        }

        public bool Remove(ISelectable selectable)
        {
            if (selectable != null && Contains(selectable))
            {
                selectable.SelectionEnded(_associatedBlockView);

                _ = _selection.Remove(selectable);

                return true;
            }

            return false;
        }

        public bool Contains(ISelectable selectable)
        {
            return selectable != null && _selection.Contains(selectable);
        }
    }
}
