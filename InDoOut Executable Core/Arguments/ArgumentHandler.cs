using InDoOut_Core.Instancing;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Executable_Core.Arguments
{
    public class ArgumentHandler : Singleton<ArgumentHandler>, IArgumentHandler
    {
        private readonly List<IArgument> _arguments = new();
        private IArgument _lastArgument = null;

        public char[] ArgumentKeyPrefixes { get; set; } = new[] { '-', '/' };
        public char[] KeyValueSeparators { get; set; } = new[] { ':', '=' };

        public IReadOnlyList<IArgument> Arguments => _arguments.AsReadOnly();

        public bool AddArgument(IArgument argument)
        {
            if (!_arguments.Contains(argument))
            {
                _arguments.Add(argument);

                return true;
            }

            return false;
        }

        public bool AddArguments(params IArgument[] arguments)
        {
            var addedAll = true;

            foreach (var argument in arguments)
            {
                addedAll = AddArgument(argument) && addedAll;
            }

            return addedAll;
        }

        public bool AddArguments(IEnumerable<IArgument> arguments)
        {
            var addedAll = true;

            foreach (var argument in arguments)
            {
                addedAll = AddArgument(argument) && addedAll;
            }

            return addedAll;
        }

        public string FormatKeyValue(IArgument argument)
        {
            if (argument?.Key != null)
            {
                var keyPart = $"{ArgumentKeyPrefixes?.FirstOrDefault().ToString() ?? ""}{argument?.Key}";
                var valuePart = argument.AllowsValue ? $"{KeyValueSeparators?.FirstOrDefault().ToString() ?? ""}[value]" : "";

                return $"{keyPart}{valuePart}";
            }

            return "";
        }

        public string FormatDescription(IArgument argument)
        {
            if (argument?.Key != null)
            {
                var keyValuePart = FormatKeyValue(argument);
                var descriptionPart = string.IsNullOrEmpty(argument.Description) ? "" : $" - {argument.Description}";

                return $"{keyValuePart}{descriptionPart}";
            }

            return "";
        }

        public void Process(params string[] rawArguments)
        {
            foreach (var rawArgument in rawArguments)
            {
                ProcessArgument(rawArgument);
            }

            if (_lastArgument != null)
            {
                _lastArgument.Trigger(this);
                _lastArgument = null;
            }
        }

        private void ProcessArgument(string rawArguments)
        {
            if (IsArgumentStringAKey(rawArguments))
            {
                ProcessArgumentKey(rawArguments);
            }
            else
            {
                ProcessArgumentValue(rawArguments);
            }
        }

        private void ProcessArgumentKey(string rawArgument)
        {
            if (_lastArgument != null)
            {
                _lastArgument.Trigger(this);
                _lastArgument = null;
            }

            var keyValue = SplitKeyValueFromArgumentString(rawArgument);
            if (!string.IsNullOrEmpty(keyValue.Key))
            {
                var argument = FindArgument(keyValue.Key);
                if (argument != null)
                {
                    if (!string.IsNullOrEmpty(keyValue.Value))
                    {
                        if (argument.AllowsValue)
                        {
                            argument.Value = keyValue.Value;
                            argument.Trigger(this);
                        }
                        else
                        {
                            throw new InvalidArgumentException($"The argument \"{argument.Key}\" doesn't allow for values to be set on it.");
                        }
                    }
                    else
                    {
                        _lastArgument = argument;
                    }
                }
                else
                {
                    throw new InvalidArgumentException($"The provided argument \"{rawArgument ?? "null"}\" does not match any known arguments.");
                }
            }
            else
            {
                throw new InvalidArgumentException($"Error processing argument as it appears to be empty. Is there a double space somewhere?");
            }
        }

        private void ProcessArgumentValue(string rawArgument)
        {
            if (_lastArgument != null)
            {
                if (_lastArgument.AllowsValue)
                {
                    _lastArgument.Value = rawArgument;
                    _lastArgument.Trigger(this);
                    _lastArgument = null;
                }
                else
                {
                    throw new InvalidArgumentException($"The argument \"{_lastArgument.Key}\" doesn't allow for values to be set on it.");
                }
            }
            else
            {
                throw new InvalidArgumentException($"The value \"{rawArgument ?? "null"}\" is not an argument, nor is it preceeded by an argument and cannot be processed.");
            }
        }

        private bool IsArgumentStringAKey(string argumentString) => ArgumentKeyPrefixes.Any(prefix => argumentString.StartsWith(prefix));

        private IArgument FindArgument(string key) => _arguments.FirstOrDefault(argument => argument.Key == key);

        private KeyValuePair<string, string> SplitKeyValueFromArgumentString(string argumentString)
        {
            var trimmedArgumentString = argumentString.TrimStart(ArgumentKeyPrefixes);
            var argumentStringKeyValuePair = new KeyValuePair<string, string>(trimmedArgumentString, "");
            var splitKeyAndValues = trimmedArgumentString.Split(KeyValueSeparators, System.StringSplitOptions.RemoveEmptyEntries);

            if (splitKeyAndValues.Length > 1)
            {
                var key = splitKeyAndValues[0];
                var value = string.Join("", splitKeyAndValues.Skip(1));

                argumentStringKeyValuePair = new KeyValuePair<string, string>(key, value);
            }

            return argumentStringKeyValuePair;
        }
    }
}
