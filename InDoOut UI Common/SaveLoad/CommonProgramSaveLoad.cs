using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Programs;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.SaveLoad
{
    public class CommonProgramSaveLoad : Singleton<CommonProgramSaveLoad>
    {
        public IProgramStorer ProgramStorer { get; } = new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance);

        public async Task<IProgram> LoadProgramDialogAsync(Window parent = null) => await ProgramSaveLoad.Instance.LoadProgramDialogAsync(ProgramHolder.Instance, ProgramStorer, parent);

        public async Task<IProgram> LoadProgramAsync(string filePath) => await ProgramSaveLoad.Instance.LoadProgramAsync(filePath, ProgramHolder.Instance, ProgramStorer);

        public async Task<bool> SaveProgramDialogAsync(IProgram program, Window parent = null) => await ProgramSaveLoad.Instance.SaveProgramDialogAsync(program, ProgramStorer, parent);

        public async Task<bool> TrySaveProgramFromMetadataAsync(IProgram program, Window parent = null) => await ProgramSaveLoad.Instance.TrySaveProgramFromMetadataAsync(program, ProgramStorer, parent);

        public async Task<bool> SaveProgramAsync(string filePath, IProgram program) => await ProgramSaveLoad.Instance.SaveProgramAsync(filePath, program, ProgramStorer);
    }
}
