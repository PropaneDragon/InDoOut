using System;
using InDoOut_Desktop.UI.Controls.Options.Types;
using InDoOut_Core.Options;

namespace InDoOut_Desktop.UI.Controls.Options
{
    public interface IOptionInterfaceFactory
    {
        ILinkedInterfaceOption GetInterfaceOptionFor(Type type);
        ILinkedInterfaceOption GetInterfaceOptionFor<OptionType>() where OptionType : IOption;
    }
}