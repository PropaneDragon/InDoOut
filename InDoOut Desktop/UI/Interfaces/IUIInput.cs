﻿using InDoOut_Core.Entities.Functions;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIInput : IUIConnectionEnd
    {
        IInput AssociatedInput { get; set; }
    }
}
