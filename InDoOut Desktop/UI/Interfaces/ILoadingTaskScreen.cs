using InDoOut_Desktop.Loading;
using InDoOut_Executable_Core.Loading;
using System.Threading.Tasks;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface ILoadingTaskScreen
    {
        Task<bool> RunTaskAsync(ILoadingTask task);
    }
}
