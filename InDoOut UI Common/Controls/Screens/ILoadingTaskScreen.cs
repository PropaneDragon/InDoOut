using InDoOut_Executable_Core.Loading;
using System.Threading.Tasks;

namespace InDoOut_UI_Common.Controls.Screens
{
    public interface ILoadingTaskScreen
    {
        Task<bool> RunTaskAsync(ILoadingTask task);
    }
}
