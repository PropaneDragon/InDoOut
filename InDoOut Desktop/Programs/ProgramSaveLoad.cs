using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Storage;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace InDoOut_Desktop.Programs
{
    internal class ProgramSaveLoad : Singleton<ProgramSaveLoad>
    {
        public IProgram LoadProgramDialog(IProgramHolder programHolder, IProgramStorer programStorer, Window parent = null)
        {
            if (programHolder != null && programStorer != null)
            {
                var openDialog = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    ValidateNames = true,
                    DefaultExt = programStorer.FileExtension,
                    Filter = $"ido Programs (*{programStorer.FileExtension})|*{programStorer.FileExtension}",
                    Title = "Select a file to load"
                };

                if (openDialog.ShowDialog(parent) ?? false)
                {
                    var fileToOpen = openDialog.FileName;
                    if (!string.IsNullOrEmpty(fileToOpen) && Path.GetExtension(fileToOpen) == programStorer.FileExtension)
                    {
                        var program = programHolder.NewProgram();
                        if (program != null)
                        {
                            programStorer.FilePath = fileToOpen;

                            var failureReports = programStorer.Load(program);
                            if (failureReports.Count == 0)
                            {
                                return program;
                            }
                            else
                            {
                                var resultStrings = failureReports.Select(report => report.Summary);
                                var canContinue = !failureReports.Any(report => report.Critical);

                                _ = MessageBox.Show(parent, $"The program couldn't be loaded due to the following errors:\n\n{string.Join('\n', resultStrings)}");
                                _ = programHolder.RemoveProgram(program);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public bool SaveProgramDialog(IProgram program, IProgramStorer programStorer, Window parent = null, string saveLocation = null)
        {
            var failureReports = new List<IFailureReport>();

            if (program != null && programStorer != null)
            {
                if (saveLocation == null)
                {
                    var saveDialog = new SaveFileDialog()
                    {
                        CheckFileExists = false,
                        CheckPathExists = true,
                        ValidateNames = true,
                        DefaultExt = programStorer.FileExtension,
                        Filter = $"ido Programs (*{programStorer.FileExtension})|*{programStorer.FileExtension}",
                        Title = "Select a file to save"
                    };

                    if (saveDialog.ShowDialog(parent) ?? false)
                    {
                        var fileToSave = saveDialog.FileName;
                        if (!string.IsNullOrEmpty(fileToSave) && Path.GetExtension(fileToSave) == programStorer.FileExtension)
                        {
                            failureReports.AddRange(SaveProgram(program, programStorer, fileToSave));
                        }
                        else
                        {
                            failureReports.Add(new FailureReport((int)SaveResult.InvalidFileName, $"The given filename to be saved ({fileToSave}) is invalid."));
                        }
                    }
                }
                else
                {
                    failureReports = SaveProgram(program, programStorer, saveLocation);
                }
            }

            if (failureReports.Count > 0)
            {
                var resultStrings = failureReports.Select(report => report.Summary);

                _ = MessageBox.Show(parent, $"The program couldn't be loaded due to the following errors:\n\n{string.Join('\n', resultStrings)}");
            }

            return failureReports.Count == 0;
        }

        private List<IFailureReport> SaveProgram(IProgram program, IProgramStorer programStorer, string saveLocation)
        {
            if (program != null && programStorer != null && !string.IsNullOrEmpty(saveLocation) && Path.GetExtension(saveLocation) == programStorer.FileExtension)
            {
                programStorer.FilePath = saveLocation;
                return programStorer.Save(program);
            }

            return new List<IFailureReport>() { new FailureReport((int)SaveResult.InvalidFileName, $"The given location ({saveLocation}), program ({program?.Id.ToString() ?? "null program"} or storer ({(programStorer != null ? "is valid" : "null")}) was invalid") };
        }
    }
}
