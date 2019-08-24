using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Storage;
using Microsoft.Win32;
using System.IO;
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

                            var loadResult = programStorer.Load(program);
                            if (loadResult == LoadResult.OK)
                            {
                                return program;
                            }
                            else
                            {
                                _ = MessageBox.Show(parent, $"The program couldn't be loaded due to an error. ({loadResult})");
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
            var saveResult = SaveResult.Unknown;

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
                            saveResult = SaveProgram(program, programStorer, fileToSave);
                        }
                        else
                        {
                            saveResult = SaveResult.InvalidFileName;
                        }
                    }
                    else
                    {
                        saveResult = SaveResult.OK;
                    }
                }
                else
                {
                    saveResult = SaveProgram(program, programStorer, saveLocation);
                }
            }

            if (saveResult != SaveResult.OK)
            {
                _ = MessageBox.Show(parent, $"The program couldn't be loaded due to an error. ({saveResult})");
            }

            return saveResult == SaveResult.OK;
        }

        private SaveResult SaveProgram(IProgram program, IProgramStorer programStorer, string saveLocation)
        {
            if (program != null && programStorer != null && !string.IsNullOrEmpty(saveLocation) && Path.GetExtension(saveLocation) == programStorer.FileExtension)
            {
                programStorer.FilePath = saveLocation;
                return programStorer.Save(program);
            }

            return SaveResult.InvalidFileName;
        }
    }
}
