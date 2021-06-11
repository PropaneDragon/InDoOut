using InDoOut_Core.Options;
using InDoOut_UI_Common.Controls.Options.Types;
using System;

namespace InDoOut_UI_Common.Controls.Options
{
    public interface IOptionInterfaceFactory
    {
        ILinkedInterfaceOption GetInterfaceOptionFor(Type type);
        ILinkedInterfaceOption GetInterfaceOptionFor<OptionType>() where OptionType : IOption;
    }
}