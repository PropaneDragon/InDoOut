using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Executable_Core.Messaging;
using System;

namespace InDoOut_Console.Messaging
{
    internal class ConsoleUserMessageSystem : AbstractUserMessageSystem
    {
        public override void ShowError(string title, string message, string details = null)
        {
            ConsoleFormatter.DrawErrorMessageLine($"{(string.IsNullOrEmpty(title) ? "" : $"{title}: ")}{message ?? ""} {details ?? ""}");
        }

        public override void ShowInformation(string title, string message, string details = null)
        {
            ConsoleFormatter.DrawInfoMessageLine($"{(string.IsNullOrEmpty(title) ? "" : $"{title}: ")}{message ?? ""} {details ?? ""}");
        }

        public override void ShowWarning(string title, string message, string details = null)
        {
            ConsoleFormatter.DrawWarningMessageLine($"{(string.IsNullOrEmpty(title) ? "" : $"{title}: ")}{message ?? ""} {details ?? ""}");
        }

        public override UserResponse? ShowQuestion(string title, string message)
        {
            ConsoleFormatter.DrawInfoMessageLine($"QUESTION:");
            ConsoleFormatter.DrawInfoMessageLine($"{(string.IsNullOrEmpty(title) ? "" : $"{title}: ")}{message ?? ""}");
            ConsoleFormatter.DrawInfoMessageLine($"Yes: y, No: n, Cancel: c");

            try
            {
                var line = Console.ReadLine()?.ToLower();
                if (!string.IsNullOrEmpty(line))
                {
                    switch (line)
                    {
                        case "y":
                        case "yes":
                        case "ya":
                        case "yeah":
                            return UserResponse.Yes;
                        case "n":
                        case "no":
                        case "nah":
                        case "nope":
                            return UserResponse.No;
                        case "c":
                        case "cancel":
                        case "escape":
                            return UserResponse.Cancel;
                    };
                }
            }
            catch { }

            return UserResponse.Cancel;
        }
    }
}
