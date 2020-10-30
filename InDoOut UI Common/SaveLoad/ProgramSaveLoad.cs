using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Programs;
using InDoOut_Executable_Core.Storage;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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
                    return await LoadProgramAsync(openDialog.FileName, programHolder, programStorer);
                }
            }

            return null;
        }

        public async Task<IProgram> LoadProgramAsync(string filePath, IProgramHolder programHolder, IProgramStorer programStorer)
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
                        try
                        {
                            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            programStorer.FileStream = stream;

                            failureReports.AddRange(await Task.Run(() => programStorer.Load(program)));

                            if (!failureReports.Any(FailureReport => FailureReport.Critical))
                            {
                                program.Metadata[ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM] = filePath;
                                program.SetName(Path.GetFileNameWithoutExtension(filePath));
                            }

                            _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, filePath);
                        }
                        catch (SecurityException)
                        {
                            failureReports.Add(new FailureReport((int)LoadResult.InsufficientPermissions, "The program couldn't be loaded due to invalid security permissions.", true));
                        }
                        catch (IOException)
                        {
                            failureReports.Add(new FailureReport((int)LoadResult.InvalidLocation, "The program couldn't be loaded due to invalid file permissions.", true));
                        }
                        catch
                        {
                            failureReports.Add(new FailureReport((int)LoadResult.InvalidFile, "The program couldn't be loaded due to an invalid file.", true));
                        }
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

            var totalReports = failureReports.Count;
            if (totalReports > 0)
            {
                var criticalReports = failureReports.Count(report => report.Critical);
                var resultStrings = failureReports.Select(report => report.Summary);
                var canContinue = criticalReports <= 0;

                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowWarning("Problems loading program", $"{totalReports} problem{(totalReports != 1 ? "s" : "")} occurred while trying to load. See below for details.", $"{string.Join("\n\n", resultStrings)}");

                if (canContinue)
                {
                    var result = UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowQuestion("Continue loading?", "As there are no critical errors, do you wish to load anyway?\n\nPlease note that there may be missing elements or other features if you choose to load.") ?? UserResponse.Yes;
                    if (result == UserResponse.Yes)
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
                    return await SaveProgramAsync(saveDialog.FileName, program, programStorer);
                }
            }

            return false;
        }

        public async Task<bool> TrySaveProgramFromMetadataAsync(IProgram program, IProgramStorer programStorer, Window parent = null)
        {
            return program != null && program.Metadata.ContainsKey(ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM)
                ? await SaveProgramAsync(program.Metadata[ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM], program, programStorer)
                : await SaveProgramDialogAsync(program, programStorer, parent);
        }

        public async Task<bool> SaveProgramAsync(string filePath, IProgram program, IProgramStorer programStorer)
        {
            var failureReports = new List<IFailureReport>();

            if (!string.IsNullOrEmpty(filePath) && programStorer != null)
            {
                if (Path.GetExtension(filePath) == programStorer.FileExtension)
                {
                    try
                    {
                        var fileMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.CreateNew;
                        using var stream = new FileStream(filePath, fileMode, FileAccess.Write);

                        programStorer.FileStream = stream;

                        failureReports.AddRange(await Task.Run(() => programStorer.Save(program)));
                    }
                    catch (SecurityException)
                    {
                        failureReports.Add(new FailureReport((int)LoadResult.InsufficientPermissions, "The program couldn't be saved due to invalid security permissions.", true));
                    }
                    catch (IOException)
                    {
                        failureReports.Add(new FailureReport((int)LoadResult.InvalidLocation, "The program couldn't be saved due to invalid file permissions.", true));
                    }
                    catch
                    {
                        failureReports.Add(new FailureReport((int)LoadResult.InvalidFile, "The program couldn't be saved due to an invalid file.", true));
                    }
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

                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowWarning("", $"The program couldn't be loaded due to the following errors:\n\n{string.Join("\n- ", resultStrings)}");
            }
            else if (program != null)
            {
                _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, filePath);
            }

            return failureReports.Count == 0;
        }
    }
}
