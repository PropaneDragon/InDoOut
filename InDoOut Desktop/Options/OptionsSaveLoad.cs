using InDoOut_Core.Instancing;
using InDoOut_Core.Options;
using InDoOut_Core.Plugins;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Desktop.Options
{
    public class OptionsSaveLoad : Singleton<OptionsSaveLoad>
    {
        private static readonly string PROGRAM_OPTIONS_SAVE_FILENAME = "IDO";

        public string ProgramOptionsFilename => $"{PROGRAM_OPTIONS_SAVE_FILENAME}{OptionsStorer?.FileExtension ?? ""}";

        public IOptionsStorer OptionsStorer { get; set; } = new OptionsJsonStorer();

        public async Task<bool> LoadAllOptionsAsync(Window parent = null)
        {
            var allSucceded = await LoadProgramOptionsAsync(parent);
            allSucceded = await LoadPluginOptionsAsync(parent) && allSucceded;

            return allSucceded;
        }

        public async Task<bool> SaveAllOptionsAsync(Window parent = null)
        {
            var allSucceded = await SaveProgramOptionsAsync(parent);
            allSucceded = await SavePluginOptionsAsync(parent) && allSucceded;

            return allSucceded;
        }

        public async Task<bool> LoadProgramOptionsAsync(Window parent = null)
        {
            return await LoadOptionsAsync(ProgramOptionsFilename, ProgramSettings.Instance.OptionHolder, parent);
        }

        public async Task<bool> LoadPluginOptionsAsync(Window parent = null)
        {
            var allSucceeded = true;

            foreach (var pluginContainer in LoadedPlugins.Instance.Plugins)
            {
                if (pluginContainer.Plugin != null)
                {
                    allSucceeded = await LoadPluginOptionsAsync(pluginContainer.Plugin, parent) && allSucceeded;
                }
            }

            return allSucceeded;
        }

        public async Task<bool> LoadPluginOptionsAsync(IPlugin plugin, Window parent = null)
        {
            return plugin != null ? await LoadOptionsAsync(GetPluginFileName(plugin), plugin?.OptionHolder, parent) : false;
        }

        public async Task<bool> SaveProgramOptionsAsync(Window parent = null)
        {
            return await SaveOptionsAsync(ProgramOptionsFilename, ProgramSettings.Instance.OptionHolder, parent);
        }

        public async Task<bool> SavePluginOptionsAsync(Window parent = null)
        {
            var allSucceeded = true;

            foreach (var pluginContainer in LoadedPlugins.Instance.Plugins)
            {
                if (pluginContainer.Plugin != null)
                {
                    allSucceeded = await SavePluginOptionsAsync(pluginContainer.Plugin, parent) && allSucceeded;
                }
            }

            return allSucceeded;
        }

        public async Task<bool> SavePluginOptionsAsync(IPlugin plugin, Window parent = null)
        {
            return plugin != null ? await SaveOptionsAsync(GetPluginFileName(plugin), plugin?.OptionHolder, parent) : false;
        }

        public async Task<bool> LoadOptionsAsync(string filePath, IOptionHolder optionHolder, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && optionHolder != null && OptionsStorer != null)
            {
                if (Path.GetExtension(filePath) == OptionsStorer.FileExtension)
                {
                    OptionsStorer.FilePath = filePath;
                    failureReports.AddRange(await Task.Run(() => OptionsStorer.Load(optionHolder)));
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

                _ = MessageBox.Show(parent, $"The options couldn't be loaded due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }

            return failureReports.Count == 0;
        }

        public async Task<bool> SaveOptionsAsync(string filePath, IOptionHolder optionHolder, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && optionHolder != null && OptionsStorer != null)
            {
                if (Path.GetExtension(filePath) == OptionsStorer.FileExtension)
                {
                    OptionsStorer.FilePath = filePath;
                    failureReports.AddRange(await Task.Run(() => OptionsStorer.Save(optionHolder)));
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

                _ = MessageBox.Show(parent, $"The options couldn't be saved due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }

            return failureReports.Count == 0;
        }

        private string GetPluginFileName(IPlugin plugin)
        {
            if (plugin != null)
            {
                return $"{plugin.SafeName}{OptionsStorer?.FileExtension ?? ""}";
            }

            return null;
        }
    }
}
