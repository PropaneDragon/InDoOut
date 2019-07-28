using System;
using System.Collections.Generic;
using System.Text;

namespace InDoOut_Desktop.Actions
{
    internal interface IDraggable
    {
        void DragStarted();
        void DragMoved();
        void DragEnded();

        bool CanDrag();
    }
}
