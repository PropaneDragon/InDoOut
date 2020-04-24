using InDoOut_Core.Instancing;
using InDoOut_Core.Options;
using InDoOut_Core.Plugins;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.SaveLoad
{
    public class OptionsSaveLoad : Singleton<OptionsSaveLoad>
    {
        private static readonly string PROGRAM_OPTIONS_SAVE_FILENAME = "IDO";

        public async Task<bool> LoadAllOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            var allSucceded = await LoadProgramOptionsAsync(optionsStorer, parent);
            allSucceded = await LoadPluginOptionsAsync(optionsStorer, parent) && allSucceded;

            return allSucceded;
        }

        public async Task<bool> SaveAllOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            var allSucceded = await SaveProgramOptionsAsync(optionsStorer, parent);
            allSucceded = await SavePluginOptionsAsync(optionsStorer, parent) && allSucceded;

            return allSucceded;
        }

        public async Task<bool> LoadProgramOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            return await LoadOptionsAsync(GetProgramFileName(optionsStorer), ProgramOptionHolder.Instance.ProgramOptions?.OptionHolder, optionsStorer, parent);
        }

        public async Task<bool> LoadPluginOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            var allSucceeded = true;

            foreach (var pluginContainer in LoadedPlugins.Instance.Plugins)
            {
                if (pluginContainer.Plugin != null)
                {
                    allSucceeded = await LoadPluginOptionsAsync(pluginContainer.Plugin, optionsStorer, parent) && allSucceeded;
                }
            }

            return allSucceeded;
        }

        public async Task<bool> LoadPluginOptionsAsync(IPlugin plugin, IOptionsStorer optionsStorer, Window parent = null)
        {
            return plugin != null ? await LoadOptionsAsync(GetPluginFileName(plugin, optionsStorer), plugin?.OptionHolder, optionsStorer, parent) : false;
        }

        public async Task<bool> SaveProgramOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            return await SaveOptionsAsync(GetProgramFileName(optionsStorer), ProgramOptionHolder.Instance.ProgramOptions?.OptionHolder, optionsStorer, parent);
        }

        public async Task<bool> SavePluginOptionsAsync(IOptionsStorer optionsStorer, Window parent = null)
        {
            var allSucceeded = true;

            foreach (var pluginContainer in LoadedPlugins.Instance.Plugins)
            {
                if (pluginContainer.Plugin != null)
                {
                    allSucceeded = await SavePluginOptionsAsync(pluginContainer.Plugin, optionsStorer, parent) && allSucceeded;
                }
            }

            return allSucceeded;
        }

        public async Task<bool> SavePluginOptionsAsync(IPlugin plugin, IOptionsStorer optionsStorer, Window parent = null)
        {
            return plugin != null ? await SaveOptionsAsync(GetPluginFileName(plugin, optionsStorer), plugin?.OptionHolder, optionsStorer, parent) : false;
        }

        public async Task<bool> LoadOptionsAsync(string filePath, IOptionHolder optionHolder, IOptionsStorer optionStorer, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && optionHolder != null && optionStorer != null)
            {
                if (Path.GetExtension(filePath) == optionStorer.FileExtension)
                {
                    optionStorer.FilePath = filePath;
                    failureReports.AddRange(await Task.Run(() => optionStorer.Load(optionHolder)));
                }
                else
                {
                    failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The options couldn't be properly loaded as the given file extension is invalid.", true));
                }
            }
            else
            {
                failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"Invalid storer, holder or path.", true));
            }

            var criticalReports = failureReports.Where(report => report.Critical);
            if (criticalReports.Count() > 0)
            {
                var resultStrings = criticalReports.Select(report => report.Summary);

                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowWarning("", $"The options couldn't be loaded due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }

            return failureReports.Count == 0;
        }

        public async Task<bool> SaveOptionsAsync(string filePath, IOptionHolder optionHolder, IOptionsStorer optionsStorer, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && optionHolder != null && optionsStorer != null)
            {
                if (Path.GetExtension(filePath) == optionsStorer.FileExtension)
                {
                    optionsStorer.FilePath = filePath;
                    failureReports.AddRange(await Task.Run(() => optionsStorer.Save(optionHolder)));
                }
                else
                {
                    failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The given options filename to be saved is invalid.", true));
                }
            }
            else
            {
                failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"Invalid storer, holder or path.", true));
            }

            var criticalReports = failureReports.Where(report => report.Critical);
            if (criticalReports.Count() > 0)
            {
                var resultStrings = criticalReports.Select(report => report.Summary);

                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowWarning("", $"The options couldn't be saved due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }

            return failureReports.Count == 0;
        }

        private string GetProgramFileName(IOptionsStorer optionsStorer) => $"{PROGRAM_OPTIONS_SAVE_FILENAME}{optionsStorer?.FileExtension ?? ""}";

        private string GetPluginFileName(IPlugin plugin, IOptionsStorer optionsStorer) => plugin != null ? $"{plugin.SafeName}{optionsStorer?.FileExtension ?? ""}" : null;
    }
}
