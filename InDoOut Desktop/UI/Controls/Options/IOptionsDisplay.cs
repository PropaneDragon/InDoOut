using System.Collections.Generic;
using InDoOut_Core.Options;

namespace InDoOut_Desktop.UI.Controls.Options
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