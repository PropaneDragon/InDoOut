using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_UI_Common.Actions
{
    public class CommonProgramDisplayRestingAction : CommonDisplayRestingAction
    {
        public ICommonProgramDisplay ProgramDisplay => Display as ICommonProgramDisplay;

        public CommonProgramDisplayRestingAction(ICommonProgramDisplay display) : base(display)
        {
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            return base.MouseLeftDown(mousePosition);
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            return base.MouseLeftMove(mousePosition);
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            return base.MouseLeftUp(mousePosition);
        }

        public override bool KeyUp(Key key)
        {
            return base.KeyUp(key);
        }
    }
}
