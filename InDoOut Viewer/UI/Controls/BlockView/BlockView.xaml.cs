using InDoOut_Networking.Entities;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Threading;
using System.Timers;
using System.Windows.Controls;

namespace InDoOut_Viewer.UI.Controls.BlockView
{
    public partial class BlockView : CommonBlockDisplay
    {
        private static readonly TimeSpan _fastUpdateInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan _slowUpdateInterval = TimeSpan.FromSeconds(1);

        private readonly System.Timers.Timer _timer = new System.Timers.Timer() { AutoReset = false, Interval = _slowUpdateInterval.TotalMilliseconds };

        protected override ScrollViewer ScrollViewer => Scroll_Content;
        protected override Canvas ElementCanvas => Canvas_Content;

        public override ISelectionManager<ISelectable> SelectionManager { get; protected set; }
        public override IActionHandler ActionHandler { get; protected set; }

        public BlockView() : base()
        {
            InitializeComponent();

            AssociatedProgram = ProgramHandler.NewProgram();
            //ActionHandler = new ActionHandler(new BlockViewRestingAction(this));
            //SelectionManager = new SelectionManager(this);
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (AssociatedProgram is INetworkedProgram networkedProgram)
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                _ = await networkedProgram.Synchronise(cancellationTokenSource.Token);

                _timer.Interval = (networkedProgram.Running ? _fastUpdateInterval : _slowUpdateInterval).TotalMilliseconds;
            }

            _timer.Start();
        }

        private void CommonBlockDisplay_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _timer.Start();
            _timer.Elapsed += Timer_Elapsed;
        }

        private void CommonBlockDisplay_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
        }
    }
}
