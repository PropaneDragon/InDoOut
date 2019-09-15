using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public interface IApiFunction : IFunction
    {
        IProperty<string> BridgeIPProperty { get; }
        IProperty<string> UserIdProperty { get; }
    }
}
