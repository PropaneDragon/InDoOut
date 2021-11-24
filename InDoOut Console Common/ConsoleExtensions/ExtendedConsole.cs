using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace InDoOut_Console_Common.ConsoleExtensions
{
    public static class ExtendedConsole
    {
        private static readonly object _writingLock = new();
        private static readonly object _writingLineLock = new();

        [Flags]
        public enum ConsoleTextStyle
        {
            None = 0,
            Bold = 1,
            Faint = 2,
            Unerlined = 4,
            Italic = 8,
            Strikethrough = 16
        }

        public enum ConsoleColourArea
        {
            Foreground,
            Background
        }

        private static readonly Dictionary<ConsoleColor, Color> _colourAssociations = new();
        private static readonly Dictionary<ConsoleTextStyle, string> _textStyleValues = new()
        {
            { ConsoleTextStyle.Bold, "1" },
            { ConsoleTextStyle.Faint, "2" },
            { ConsoleTextStyle.Unerlined, "4" },
            { ConsoleTextStyle.Italic, "3" },
            { ConsoleTextStyle.Strikethrough, "9" },
        };

        private static object _lastObject = null;
        private static ConsoleColor _originalForeground = ConsoleColor.White;
        private static ConsoleColor _originalBackground = ConsoleColor.Black;

        private static string FormattingStartString => HighColourMode ? "\x1b[" : "";
        private static string FormattingEndString => HighColourMode ? "m" : "";

        public static bool HighColourMode { get; set; } = true;
        public static bool ResetColoursAfterWrite { get; set; } = true;

        public static void SetUp()
        {
            _originalForeground = Console.ForegroundColor;
            _originalBackground = Console.BackgroundColor;

            foreach (int colourValue in Enum.GetValues(typeof(ConsoleColor)))
            {
                var colourName = Enum.GetNames(typeof(ConsoleColor))[colourValue];
                var createdColour = Color.FromName(colourName);

                _colourAssociations.Add((ConsoleColor)colourValue, createdColour);
            }
        }

        public static void Write(params object[] items)
        {
            lock (_writingLock)
            {
                _lastObject = null;

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        if (item is ConsoleTextStyle textStyle)
                        {
                            WriteStyleTag(textStyle);
                        }
                        else if (item is Color colour)
                        {
                            if (colour != Color.Transparent)
                            {
                                WriteColourTag(colour, _lastObject is ConsoleColourArea area ? area : ConsoleColourArea.Foreground);
                            }
                        }
                        else if (item is ConsoleColor consoleColour)
                        {
                            WriteColourTag(consoleColour, _lastObject is ConsoleColourArea area ? area : ConsoleColourArea.Foreground);
                        }
                        else if (item is not ConsoleColourArea)
                        {
                            Console.Write(item);
                        }
                    }

                    _lastObject = item;
                }

                WriteLineFormattingReset();
            }
        }

        public static void WriteLine(params object[] items)
        {
            lock (_writingLineLock)
            {
                Write(items);
                Console.WriteLine();
            }
        }

        private static void WriteStyleTag(ConsoleTextStyle style)
        {
            if (HighColourMode)
            {
                var enumValues = Enum.GetValues(typeof(ConsoleTextStyle));

                foreach (var enumValue in enumValues)
                {
                    var @enum = (ConsoleTextStyle)enumValue;

                    if (style.HasFlag(@enum) && _textStyleValues.TryGetValue(@enum, out var formatValue))
                    {
                        Console.Write(EncaseInFormattingString(formatValue));
                    }
                }
            }
        }

        private static void WriteColourTag(Color colour, ConsoleColourArea area = ConsoleColourArea.Foreground)
        {
            if (HighColourMode)
            {
                Console.Write(area == ConsoleColourArea.Foreground ? GetHighColourForegroundString(colour) : GetHighColourBackgroundString(colour));
            }
            else
            {
                var closestColour = GetClosestConsoleColour(colour);

                if (area == ConsoleColourArea.Foreground)
                {
                    Console.ForegroundColor = closestColour;
                }
                else
                {
                    Console.BackgroundColor = closestColour;
                }
            }
        }

        private static void WriteColourTag(ConsoleColor colour, ConsoleColourArea area = ConsoleColourArea.Foreground)
        {
            var asRgb = GetColourForConsoleColour(colour);

            WriteColourTag(asRgb, area);
        }

        private static void WriteLineFormattingReset()
        {
            if (ResetColoursAfterWrite)
            {
                if (HighColourMode)
                {
                    Console.Write(EncaseInFormattingString("0"));
                }
                else
                {
                    Console.ForegroundColor = _originalForeground;
                    Console.BackgroundColor = _originalBackground;
                }
            }
        }

        private static ConsoleColor GetClosestConsoleColour(Color colour)
        {
            if (_colourAssociations.Count > 0)
            {
                var orderedKeyValues = _colourAssociations.OrderBy(consoleColur => Math.Pow((colour.GetHue() - consoleColur.Value.GetHue()) / 360d, 2) + Math.Pow(colour.GetSaturation() - consoleColur.Value.GetSaturation(), 2) + Math.Pow(colour.GetBrightness() - consoleColur.Value.GetBrightness(), 2));
                return orderedKeyValues.First().Key;
            }

            return ConsoleColor.White;
        }

        private static string EncaseInFormattingString(string @string) => $"{FormattingStartString}{@string}{FormattingEndString}";
        private static string GetHighColourForegroundString(Color colour) => EncaseInFormattingString($"38;2;{colour.R};{colour.G};{colour.B}");
        private static string GetHighColourBackgroundString(Color colour) => EncaseInFormattingString($"48;2;{colour.R};{colour.G};{colour.B}");
        private static Color GetColourForConsoleColour(ConsoleColor colour) => _colourAssociations.TryGetValue(colour, out var associatedColour) ? associatedColour : Color.FromArgb(255, 255, 255);
    }
}
