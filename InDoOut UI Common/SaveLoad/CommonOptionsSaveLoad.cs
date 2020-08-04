using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using System.Threading.Tasks;

namespace InDoOut_UI_Common.SaveLoad
{
    public class CommonOptionsSaveLoad : Singleton<CommonOptionsSaveLoad>
    {
        public IOptionsStorer OptionsStorer { get; } = new OptionsJsonStorer();

        public async Task<bool> SaveAllOptionsAsync() => await OptionsSaveLoad.Instance.SaveAllOptionsAsync(OptionsStorer);
        public async Task<bool> SaveProgramOptionsAsync() => await OptionsSaveLoad.Instance.SaveProgramOptionsAsync(OptionsStorer);
        public async Task<bool> SavePluginOptionsAsync() => await OptionsSaveLoad.Instance.SavePluginOptionsAsync(OptionsStorer);

        public async Task<bool> LoadAllOptionsAsync() => await OptionsSaveLoad.Instance.LoadAllOptionsAsync(OptionsStorer);
        public async Task<bool> LoadProgramOptionsAsync() => await OptionsSaveLoad.Instance.LoadProgramOptionsAsync(OptionsStorer);
        public async Task<bool> LoadPluginOptionsAsync() => await OptionsSaveLoad.Instance.LoadPluginOptionsAsync(OptionsStorer);
    }
}
