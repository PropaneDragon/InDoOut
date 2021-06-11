using InDoOut_UI_Common.Actions;
using System.Windows;

namespace InDoOut_Desktop_Tests
{
    internal class TestActionMain : Action
    {
        public Action NextAction { get; set; } = null;

        public bool Created { get; set; } = false;
        public bool Returns { get; set; } = false;
        public bool TriggeredMouseLeftDown { get; set; } = false;
        public bool TriggeredMouseLeftUp { get; set; } = false;
        public bool TriggeredMouseLeftMove { get; set; } = false;
        public bool TriggeredMouseRightDown { get; set; } = false;
        public bool TriggeredMouseRightUp { get; set; } = false;
        public bool TriggeredMouseRightMove { get; set; } = false;

        public TestActionMain()
        {
            Created = true;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            TriggeredMouseLeftDown = true;
            Finish(NextAction);

            return Returns;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            TriggeredMouseLeftMove = true;
            Finish(NextAction);

            return Returns;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            TriggeredMouseLeftUp = true;
            Finish(NextAction);

            return Returns;
        }

        public override bool MouseRightDown(Point mousePosition)
        {
            TriggeredMouseRightDown = true;
            Finish(NextAction);

            return Returns;
        }

        public override bool MouseRightMove(Point mousePosition)
        {
            TriggeredMouseRightMove = true;
            Finish(NextAction);

            return Returns;
        }

        public override bool MouseRightUp(Point mousePosition)
        {
            TriggeredMouseRightUp = true;
            Finish(NextAction);

            return Returns;
        }
    }
}
