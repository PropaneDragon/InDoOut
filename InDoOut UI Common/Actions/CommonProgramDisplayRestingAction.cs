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

        public override bool MouseLeftDown(Point mousePosition) => base.MouseLeftDown(mousePosition);

        public override bool MouseLeftMove(Point mousePosition) => base.MouseLeftMove(mousePosition);

        public override bool MouseLeftUp(Point mousePosition) => base.MouseLeftUp(mousePosition);

        public override bool MouseWheel(int delta) => base.MouseWheel(delta);

        public override bool KeyUp(Key key) => base.KeyUp(key);
    }
}
