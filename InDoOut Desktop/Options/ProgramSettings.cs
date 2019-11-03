using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Core.Options;
using InDoOut_Core.Options.Types;
using System.Linq;

namespace InDoOut_Desktop.Options
{
    internal class ProgramSettings : Singleton<ProgramSettings>
    {
        public CheckableOption StartWithComputer { get; } = new CheckableOption("Start with computer", "Starts IDO when the computer starts.", true);
        public CheckableOption StartInBackground { get; } = new CheckableOption("Start in the background", "Starts IDO minimised.", false);

        public IOptionHolder OptionHolder { get; } = new OptionHolder();
        
        public ProgramSettings()
        {
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
        }
    }
}
