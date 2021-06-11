using InDoOut_Core.Options;
using System.Collections.Generic;

namespace InDoOut_UI_Common.Controls.Options
{
    public interface IOptionsDisplay
    {
        string Title { get; }
        List<IOption> AssociatedOptions { get; set; }

        void CommitChanges();
        void InitializeComponent();
        void PopulateForOptions(List<IOption> options);
    }
}