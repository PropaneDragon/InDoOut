using InDoOut_Plugins.Options;
using System;

namespace InDoOut_Desktop.UI.Controls.Options.Types
{
    public interface ILinkedInterfaceOption
    {
        bool UpdateFromOption(IOption option);
        bool UpdateOptionValue(IOption option);
    }
}
