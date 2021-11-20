using InDoOut_UI_Common.Actions;
using InDoOut_Viewer.UI.Controls.BlockView;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Viewer.Actions
{
    public class BlockViewRestingAction : Action
    {
        private readonly BlockView _blockView = null;
        private readonly ActionHandler _commonDisplayActions;

        public BlockViewRestingAction(BlockView blockView)
        {
            _blockView = blockView;
            _commonDisplayActions = new ActionHandler(new CommonProgramDisplayRestingAction(_blockView, CommonDisplayRestingAction.Feature.Scrolling));
        }

        public override bool MouseRightUp(Point mousePosition) => _commonDisplayActions?.MouseRightUp(mousePosition) ?? false;

        public override bool MouseDoubleClick(Point mousePosition) => _commonDisplayActions?.MouseDoubleClick(mousePosition) ?? false;

        public override bool MouseRightMove(Point mousePosition) => _commonDisplayActions?.MouseRightMove(mousePosition) ?? false;

        public override bool MouseRightDown(Point mousePosition) => _commonDisplayActions?.MouseRightDown(mousePosition) ?? false;

        public override bool MouseNoMove(Point mousePosition) => _commonDisplayActions?.MouseNoMove(mousePosition) ?? false;

        public override bool MouseLeftDown(Point mousePosition) => _commonDisplayActions?.MouseLeftDown(mousePosition) ?? false;

        public override bool MouseLeftMove(Point mousePosition) => _commonDisplayActions?.MouseLeftMove(mousePosition) ?? false;

        public override bool MouseLeftUp(Point mousePosition) => _commonDisplayActions?.MouseLeftUp(mousePosition) ?? false;

        public override bool MouseWheel(int delta) => _commonDisplayActions?.MouseWheel(delta) ?? false;

        public override bool KeyUp(Key key) => _commonDisplayActions?.KeyUp(key) ?? false;

        public override bool KeyDown(Key key) => _commonDisplayActions?.KeyDown(key) ?? false;
    }
}
