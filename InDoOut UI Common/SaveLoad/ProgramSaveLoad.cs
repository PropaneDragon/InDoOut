using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Programs;
using InDoOut_Executable_Core.Storage;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.SaveLoad
{
    public class ProgramSaveLoad : Singleton<ProgramSaveLoad>
    {
        public async Task<IProgram> LoadProgramDialogAsync(IProgramHolder programHolder, IProgramStorer programStorer, Window parent = null)
        {
            if (programHolder != null && programStorer != null)
            {
                var openDialog = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    ValidateNames = true,
                    DefaultExt = programStorer.FileExtension,
                    Filter = $"{programStorer.FileReadableName} (*{programStorer.FileExtension})|*{programStorer.FileExtension}",
                    Title = "Select a file to load"
                };

                if (openDialog.ShowDialog(parent) ?? false)
                {
                    return await LoadProgramAsync(openDialog.FileName, programHolder, programStorer, parent);
                }
            }

            return null;
        }

        public async Task<IProgram> LoadProgramAsync(string filePath, IProgramHolder programHolder, IProgramStorer programStorer, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            IProgram program = null;

            if (!string.IsNullOrEmpty(filePath) && programHolder != null && programStorer != null)
            {
                if (Path.GetExtension(filePath) == programStorer.FileExtension)
                {
                    program = programHolder.NewProgram();
                    if (program != null)
                    {
                        programStorer.FilePath = filePath;

                        failureReports.AddRange(await Task.Run(() => programStorer.Load(program)));

                        _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, filePath);
                    }
                    else
                    {
                        failureReports.Add(new FailureReport((int)LoadResult.MissingData, "A program could not be created to attach to due to an unknown issue.", true));
                    }
                }
                else
                {
                    failureReports.Add(new FailureReport((int)LoadResult.InvalidExtension, "The file extension given is invalid. Please provide a valid file.", true));
                }
            }
            else
            {
                failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The given location ({filePath}), holder ({(programStorer != null ? "is valid" : "null")}) or storer ({(programStorer != null ? "is valid" : "null")}) was invalid"));
            }

            if (failureReports.Count > 0)
            {
                var resultStrings = failureReports.Select(report => report.Summary);
                var canContinue = !failureReports.Any(report => report.Critical);

                _ = MessageBox.Show(parent, $"The following errors occurred trying to load the program:\n\n{string.Join("\n- ", resultStrings)}");

                if (canContinue)
                {
                    var result = MessageBox.Show(parent, "As there are no critical errors, do you wish to load anyway?\n\nPlease note that there may be missing elements or other features if you choose to load.", "Load anyway?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        return program;
                    }
                }
            }
            else
            {
                return program;
            }

            if (program != null)
            {
                _ = programHolder.RemoveProgram(program);
            }

            return null;
        }

        public async Task<bool> SaveProgramDialogAsync(IProgram program, IProgramStorer programStorer, Window parent = null)
        {
            if (program != null && programStorer != null)
            {
                var saveDialog = new SaveFileDialog()
                {
                    CheckFileExists = false,
                    CheckPathExists = true,
                    ValidateNames = true,
                    DefaultExt = programStorer.FileExtension,
                    Filter = $"{programStorer.FileReadableName} (*{programStorer.FileExtension})|*{programStorer.FileExtension}",
                    Title = "Select a file to save"
                };

                if (saveDialog.ShowDialog(parent) ?? false)
                {
                    return await SaveProgramAsync(saveDialog.FileName, program, programStorer, parent);
                }
            }

            return false;
        }

        public async Task<bool> TrySaveProgramFromMetadataAsync(IProgram program, IProgramStorer programStorer, Window parent = null)
        {
            return program != null && program.Metadata.ContainsKey(ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM)
                ? await SaveProgramAsync(program.Metadata[ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM], program, programStorer, parent)
                : await SaveProgramDialogAsync(program, programStorer, parent);
        }

        public async Task<bool> SaveProgramAsync(string filePath, IProgram program, IProgramStorer programStorer, Window parent = null)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && programStorer != null)
            {
                if (Path.GetExtension(filePath) == programStorer.FileExtension)
                {
                    programStorer.FilePath = filePath;
                    failureReports.AddRange(await Task.Run(() => programStorer.Save(program)));
                }
                else
                {
                    failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The given filename to be saved ({filePath}) is invalid."));
                }
            }
            else
            {
                failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The given location ({filePath}), program ({program?.Id.ToString() ?? "null program"} or storer ({(programStorer != null ? "is valid" : "null")}) was invalid"));
            }

            if (failureReports.Count > 0)
            {
                var resultStrings = failureReports.Select(report => report.Summary);

                _ = MessageBox.Show(parent, $"The program couldn't be loaded due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }
            else if (program != null)
            {
                _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, filePath);
            }

            return failureReports.Count == 0;
        }
    }
}
