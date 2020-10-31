using InDoOut_Core.Instancing;

namespace InDoOut_Executable_Core.Options
{
    public class ProgramOptionsHolder : Singleton<ProgramOptionsHolder>
    {
        public IAbstractProgramOptions ProgramOptions { get; set; } = null;

        public T Get<T>() where T : class, IAbstractProgramOptions => ProgramOptions as T;
    }
}
