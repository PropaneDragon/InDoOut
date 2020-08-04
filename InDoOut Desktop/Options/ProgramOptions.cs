using InDoOut_Core.Basic;
using InDoOut_Core.Logging;
using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_UI_Common.Controls.TaskManager;
using Microsoft.Win32;

namespace InDoOut_Desktop.Options
{
    internal class ProgramOptions : AbstractProgramOptions
    {
        //public CheckableOption StartWithComputerAll { get; } = new CheckableOption("Start with computer (All users)", "Starts IDO when the computer starts. This sets the program to start for all users, so regardless of what user logs on the program will start.", true);
        public CheckableOption StartWithComputerCurrent { get; } = new CheckableOption("Start with computer (current user)", "Starts IDO when the computer starts. This sets the program to start for only you. Other users of this machine are unaffected.", false);
        public CheckableOption StartInBackground { get; } = new CheckableOption("Start in the background", "Starts IDO minimised.", false);
        public CheckableOption AllowLogging { get; } = new CheckableOption("Logging", "Allows IDO to log all events to a file for debugging.", true);
        public HiddenListOption StartupPrograms { get; } = new HiddenListOption(TaskItem.TASK_STARTUP_PROGRAM_OPTION_NAME);

        public ProgramOptions() : base()
        {
        }

        protected override void HookOptions()
        {
            //StartWithComputerAll.OnValueChanged += StartWithComputer_OnValueChanged;
            StartWithComputerCurrent.OnValueChanged += StartWithComputer_OnValueChanged;
            AllowLogging.OnValueChanged += AllowLogging_OnValueChanged;
        }

        private void AllowLogging_OnValueChanged(object sender, ValueChangedEvent e)
        {
            Log.Instance.Enabled = e.Value.ValueAs(true);
        }

        private void StartWithComputer_OnValueChanged(object sender, ValueChangedEvent e)
        {
            Log.Instance.Info("Setting program startup registry item");

            try
            {
                SetStartWithComputer(Registry.CurrentUser, StartWithComputerCurrent.Value);
                //SetStartWithComputer(Registry.LocalMachine, StartWithComputerAll.Value);
            }
            catch
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Unable to set program startup option", "Couldn't set the option to start the application on startup due to an issue with the registry setting.\n\nYou may not have permission to set this option.");
            }
        }

        private void SetStartWithComputer(RegistryKey topLevel, bool shouldStart)
        {
            if (topLevel != null)
            {
                var runKey = topLevel.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (shouldStart)
                {
                    runKey.SetValue("InDoOut", StandardLocations.Instance.GetPathTo(Location.ApplicationExecutable));
                }
                else
                {
                    runKey.DeleteValue("InDoOut", false);
                }
            }
        }
    }
}
