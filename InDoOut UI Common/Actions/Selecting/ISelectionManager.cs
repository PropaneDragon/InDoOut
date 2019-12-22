using System.Collections.Generic;

namespace InDoOut_UI_Common.Actions.Selecting
{
    public interface ISelectionManager<SelectableType> where SelectableType : ISelectable
    {
        List<SelectableType> Selection { get; }

        void Clear();

        bool Add(SelectableType selectable, bool toggleIfAlreadyInserted = false);
        bool Set(SelectableType selectable, bool toggleIfAlreadyInserted = false);
        bool Remove(SelectableType selectable);
        bool Contains(SelectableType selectable);
    }
}