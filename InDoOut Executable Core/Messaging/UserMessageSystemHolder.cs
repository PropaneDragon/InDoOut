using InDoOut_Core.Instancing;

namespace InDoOut_Executable_Core.Messaging
{
    public class UserMessageSystemHolder : Singleton<UserMessageSystemHolder>
    {
        public IAbstractUserMessageSystem CurrentUserMessageSystem { get; set; } = new NullUserMessageSystem();
    }
}
