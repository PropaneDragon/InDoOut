using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop_API.Plugins;
using System.Collections.Generic;

namespace InDoOut_Desktop.Plugins
{
    internal interface IPluginContainer
    {
        bool Valid { get; }

        IPlugin Plugin { get; }

        List<IFunction> Functions { get; }

        bool Initialise();
    }
}
