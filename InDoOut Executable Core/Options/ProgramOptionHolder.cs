using InDoOut_Core.Instancing;

namespace InDoOut_Executable_Core.Options
{
    public class ProgramOptionHolder : Singleton<ProgramOptionHolder>
    {
        public IAbstractProgramOptions ProgramOptions { get; set; } = null;

        public T Get<T>() where T : class, IAbstractProgramOptions
        {
            return ProgramOptions as T;
        }
    }
}
