using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;

namespace InDoOut_Desktop.Options
{
    public class OptionsSaveLoad : Singleton<OptionsSaveLoad>
    {
        public IOptionsStorer OptionsStorer { get; set; } = new OptionsJsonStorer();


    }
}
