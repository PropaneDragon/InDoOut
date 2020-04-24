using InDoOut_Core.Options;

namespace InDoOut_UI_Common.Controls.Options.Types
{
    public interface ILinkedInterfaceOption
    {
        bool UpdateFromOption(IOption option);
        bool UpdateOptionValue(IOption option);
    }
}
