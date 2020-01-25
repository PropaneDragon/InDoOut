using System;

namespace InDoOut_UI_Common.Controls.Search
{
    internal interface ISearch
    {
        event EventHandler<SearchArgs> SearchRequested;
    }
}
