using InDoOut_Core.Instancing;
using InDoOut_Display_Core.Elements;
using InDoOut_Plugins.Loaders;
using System.Collections.Generic;

namespace InDoOut_Display.DisplayElements
{
    internal class DisplayElementCache : Singleton<DisplayElementCache>
    {
        private readonly List<IDisplayElement> _displayElements = new List<IDisplayElement>();

        public void RefreshCache()
        {
            var plugins = LoadedPlugins.Instance.Plugins;
        }
    }
}
