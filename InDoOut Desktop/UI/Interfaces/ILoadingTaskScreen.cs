﻿using InDoOut_Desktop.Loading;
using System.Threading.Tasks;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface ILoadingTaskScreen
    {
        Task<bool> RunTaskAsync(ILoadingTask task);
    }
}