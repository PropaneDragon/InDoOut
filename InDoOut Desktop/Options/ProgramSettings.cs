﻿using InDoOut_Core.Basic;
using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Core.Options;
using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Messaging;
using Microsoft.Win32;
using System.Linq;

namespace InDoOut_Desktop.Options
{
    internal class ProgramSettings : Singleton<ProgramSettings>
    {
        //public CheckableOption StartWithComputerAll { get; } = new CheckableOption("Start with computer (All users)", "Starts IDO when the computer starts. This sets the program to start for all users, so regardless of what user logs on the program will start.", true);
        public CheckableOption StartWithComputerCurrent { get; } = new CheckableOption("Start with computer (Current user)", "Starts IDO when the computer starts. This sets the program to start for only you. Other users of this machine are unaffected.", false);
        public CheckableOption StartInBackground { get; } = new CheckableOption("Start in the background", "Starts IDO minimised.", false);

        public IOptionHolder OptionHolder { get; } = new OptionHolder();
        
        public ProgramSettings()
        {
        }

        public void RegisterOptions()
        {
            Log.Instance.Header("Automatically registering options for program settings.");

            var validProperties = GetType().GetProperties().Where(property => typeof(IOption).IsAssignableFrom(property.PropertyType));
            foreach (var validProperty in validProperties)
            {
                var getterMethod = validProperty.GetGetMethod(true);
                if (getterMethod != null)
                {
                    var potentialOption = getterMethod.Invoke(this, null);
                    if (potentialOption is IOption option)
                    {
                        _ = OptionHolder.RegisterOption(option);
                    }
                }
            }

            HookOptions();
        }

        private void HookOptions()
        {
            //StartWithComputerAll.OnValueChanged += StartWithComputer_OnValueChanged;
            StartWithComputerCurrent.OnValueChanged += StartWithComputer_OnValueChanged;
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