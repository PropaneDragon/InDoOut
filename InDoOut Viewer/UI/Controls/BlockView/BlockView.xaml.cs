using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_Viewer.Actions;
using System.Windows.Controls;

namespace InDoOut_Viewer.UI.Controls.BlockView
{
    public partial class BlockView : CommonBlockDisplay
    {
        protected override ScrollViewer ScrollViewer => Scroll_Content;
        protected override Canvas ElementCanvas => Canvas_Content;

        public override ISelectionManager<ISelectable> SelectionManager { get; protected set; }
        public override IActionHandler ActionHandler { get; protected set; }

        public BlockView() : base()
        {
            InitializeComponent();

            AssociatedProgram = ProgramHandler.NewProgram();
            ActionHandler = new ActionHandler(new BlockViewRestingAction(this));

            BlockView_Overview.Display = this;
        }
    }
}
