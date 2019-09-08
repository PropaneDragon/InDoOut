using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Desktop.Display.Selection
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