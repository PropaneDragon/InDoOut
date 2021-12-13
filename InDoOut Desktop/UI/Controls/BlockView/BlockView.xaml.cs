using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.Actions.Selecting;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Executable_Core.Messaging;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : CommonBlockDisplay, IBlockView
    {
        protected override ScrollViewer ScrollViewer => Scroll_Content;
        protected override Canvas ElementCanvas => Canvas_Content;

        public override ISelectionManager<ISelectable> SelectionManager { get; protected set; }
        public override IActionHandler ActionHandler { get; protected set; }

        public BlockView() : base()
        {
            InitializeComponent();

            AssociatedProgram = ProgramHandler.NewProgram();
            SelectionManager = new SelectionManager(this);
            ActionHandler = new ActionHandler(new BlockViewRestingAction(this));

            BlockView_Overview.Display = this;
        }

        private async void Scroll_Content_Drop(object sender, System.Windows.DragEventArgs e)
        {
            var formats = e.Data.GetFormats().ToList();
            if (formats.Contains("Function"))
            {
                if (e.Data.GetData("Function") is IFunction dataFunction)
                {
                    var functionType = dataFunction.GetType();
                    var functionBuilder = new InstanceBuilder<IFunction>();
                    var function = await Task.Run(() => functionBuilder.BuildInstance(functionType));

                    if (function != null)
                    {
                        var uiFunction = FunctionCreator?.Create(function);
                        if (uiFunction == null)
                        {
                            Log.Instance.Error("UI Function for ", function, " couldn't be created on the interface");
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function doesn't appear to be able to be placed in the current program.");
                        }
                    }
                    else
                    {
                        Log.Instance.Error("Couldn't build a function for ", functionType, " to place on the interface");
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function couldn't be created and can't be placed in the current program.");
                    }
                }
            }
        }
    }
}
