﻿namespace InDoOut_Desktop.UI.Interfaces
{
    public interface ITaskItem
    {
        IBlockView BlockView { get; }

        void UpdateSnapshot();
    }
}