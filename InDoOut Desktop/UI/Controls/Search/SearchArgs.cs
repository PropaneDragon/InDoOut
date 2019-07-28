using System;

namespace InDoOut_Desktop.UI.Controls.Search
{
    public class SearchArgs : EventArgs
    {
        public string Query { get; private set; } = null;

        public SearchArgs(string query)
        {
            Query = query;
        }
    }
}
