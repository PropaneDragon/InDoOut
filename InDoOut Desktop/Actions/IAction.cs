using System;
using System.Windows;

namespace InDoOut_Desktop.Actions
{
    internal interface IAction
    {
        event EventHandler<ActionCompleteEventArgs> ActionComplete;

        bool MouseNoMove(Point mousePosition);
        bool MouseLeftMove(Point mousePosition);
        bool MouseLeftDown(Point mousePosition);
        bool MouseLeftUp(Point mousePosition);
        bool MouseRightMove(Point mousePosition);
        bool MouseRightDown(Point mousePosition);
        bool MouseRightUp(Point mousePosition);
    }
}
