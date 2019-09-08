using System.Collections.Generic;

namespace InDoOut_Desktop.Actions.Selecting
{
    public interface ISelectionManager
    {
        List<ISelectable> Selection { get; }

        void Clear();

        bool Add(ISelectable selectable);
        bool Set(ISelectable selectable);
        bool Remove(ISelectable selectable);
        bool Contains(ISelectable selectable);
    }
}