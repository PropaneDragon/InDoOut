using InDoOut_Core.Logging;
using InDoOut_Core.Options;
using System.Linq;

namespace InDoOut_Executable_Core.Options
{
    public abstract class AbstractProgramOptions : IAbstractProgramOptions
    {
        public IOptionHolder OptionHolder { get; } = new OptionHolder();

        public AbstractProgramOptions()
        {
            RegisterOptions();
        }

        public void RegisterOptions()
        {
            Log.Instance.Header("Automatically registering options for program settings.");

            var validProperties = GetType().GetProperties().Where(property => typeof(IOption).IsAssignableFrom(property.PropertyType));
            foreach (var validProperty in validProperties)
            {
                var getterMethod = validProperty.GetGetMethod(true);
                if (getterMethod != null)
                {
                    var potentialOption = getterMethod.Invoke(this, null);
                    if (potentialOption is IOption option)
                    {
                        _ = OptionHolder.RegisterOption(option);
                    }
                }
            }

            HookOptions();
        }

        protected abstract void HookOptions();
    }
}
