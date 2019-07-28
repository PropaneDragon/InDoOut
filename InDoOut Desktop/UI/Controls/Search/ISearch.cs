using System;

namespace InDoOut_Desktop.UI.Controls.Search
{
    internal interface ISearch
    {
        event EventHandler<SearchArgs> OnSearchRequested;
    }
}
