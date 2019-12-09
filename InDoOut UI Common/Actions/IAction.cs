using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public interface IAction
    {
        event EventHandler<ActionCompleteEventArgs> ActionComplete;

        bool MouseNoMove(Point mousePosition);
        bool MouseLeftMove(Point mousePosition);
        bool MouseLeftDown(Point mousePosition);
        bool MouseLeftUp(Point mousePosition);
        bool MouseRightMove(Point mousePosition);
        bool MouseRightDown(Point mousePosition);
        bool MouseRightUp(Point mousePosition);
        bool MouseDoubleClick(Point mousePosition);
        bool KeyDown(Key key);
        bool KeyUp(Key key);
    }
}
