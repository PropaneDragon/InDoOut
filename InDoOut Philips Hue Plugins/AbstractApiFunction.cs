using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public abstract class AbstractApiFunction : Function, IApiFunction
    {
        public IProperty<string> BridgeIPProperty { get; private set; } = null;
        public IProperty<string> UserIdProperty { get; private set; } = null;

        public AbstractApiFunction()
        {
            BridgeIPProperty = AddProperty(new Property<string>("Bridge IP", "The IP address of the bridge to use to control the lights. Make sure you've logged in first.", true));
            UserIdProperty = AddProperty(new Property<string>("User ID", "The user ID given from the bridge.", true));
        }
    }
}
