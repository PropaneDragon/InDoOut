namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// A loop function that encapsulates all processing for looping through
    /// a series of indexes and populating data for each of them.
    /// </summary>
    public abstract class LoopFunction : Function, ILoopFunction
    {
        private bool _startedBefore = false;
        private int _index = 0;

        private IInput _inputFirstItem = null;

        private IOutput _outputNewItem = null;
        private IOutput _outputComplete = null;

        private IResult _resultCurrentIndex = null;

        /// <summary>
        /// Creates a basic loop function.
        /// </summary>
        public LoopFunction()
        {
            _inputFirstItem = CreateInput("First item");
            _ = CreateInput("Next item");

            _outputNewItem = CreateOutput("New item");
            _outputComplete = CreateOutput("Complete");

            _resultCurrentIndex = AddResult(new Result("Current index", "The index that is currently being processed.", "0"));
        }

        /// <summary>
        /// When the loop function has been triggered.
        /// </summary>
        /// <param name="triggeredBy">The input that triggered the loop.</param>
        /// <returns>The output to trigger on completion.</returns>
        protected override IOutput Started(IInput triggeredBy)
        {
            if (!StopRequested)
            {
                if (triggeredBy == null || triggeredBy == _inputFirstItem || !_startedBefore)
                {
                    _startedBefore = true;
                    _index = 0;

                    PreprocessItems();
                }

                if (!StopRequested && PopulateItemDataForIndex(_index))
                {
                    _ = _resultCurrentIndex.ValueFrom(_index);
                    ++_index;

                    return _outputNewItem;
                }

                AllItemsComplete();
            }

            return _outputComplete;
        }

        /// <summary>
        /// Sets up all items before procesisng the first index.
        /// </summary>
        protected abstract void PreprocessItems();

        /// <summary>
        /// Tears down all items after processing is complete.
        /// </summary>
        protected abstract void AllItemsComplete();

        /// <summary>
        /// Populates item data for a specific index and returns whether there are
        /// more indexes to fetch.
        /// </summary>
        /// <param name="index">The index to process.</param>
        /// <returns>Whether there are more indexes to fetch.</returns>
        protected abstract bool PopulateItemDataForIndex(int index);
    }
}
