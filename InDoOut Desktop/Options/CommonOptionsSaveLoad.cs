using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using InDoOut_UI_Common.SaveLoad;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Desktop.Options
{
    public class CommonOptionsSaveLoad : Singleton<CommonOptionsSaveLoad>
    {
        public IOptionsStorer OptionsStorer { get; } = new OptionsJsonStorer();

        public async Task<bool> SaveAllOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.SaveAllOptionsAsync(OptionsStorer, parent);
        public async Task<bool> SaveProgramOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.SaveProgramOptionsAsync(OptionsStorer, parent);
        public async Task<bool> SavePluginOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.SavePluginOptionsAsync(OptionsStorer, parent);

        public async Task<bool> LoadAllOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.LoadAllOptionsAsync(OptionsStorer, parent);
        public async Task<bool> LoadProgramOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.LoadProgramOptionsAsync(OptionsStorer, parent);
        public async Task<bool> LoadPluginOptionsAsync(Window parent = null) => await OptionsSaveLoad.Instance.LoadPluginOptionsAsync(OptionsStorer, parent);
    }
}
