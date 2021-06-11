using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace InDoOut_Console_Common.ConsoleExtensions
{
    public static class ConsoleFormatter
    {
        public static Color RedPastel { get; set; } = Color.FromArgb(212, 102, 102);
        public static Color Red { get; set; } = Color.FromArgb(212, 53, 53);
        public static Color OrangePastel { get; set; } = Color.FromArgb(212, 154, 102);
        public static Color Orange { get; set; } = Color.FromArgb(212, 123, 53);
        public static Color YellowPastel { get; set; } = Color.FromArgb(212, 201, 102);
        public static Color Yellow { get; set; } = Color.FromArgb(212, 197, 53);
        public static Color GreenPastel { get; set; } = Color.FromArgb(146, 212, 83);
        public static Color Green { get; set; } = Color.FromArgb(69, 212, 53);
        public static Color BluePastel { get; set; } = Color.FromArgb(102, 146, 212);
        public static Color Blue { get; set; } = Color.FromArgb(53, 149, 212);
        public static Color PurplePastel { get; set; } = Color.FromArgb(164, 102, 212);
        public static Color Purple { get; set; } = Color.FromArgb(164, 53, 212);

        public static Color Primary { get; set; } = Color.FromArgb(200, 200, 200);
        public static Color Secondary { get; set; } = Color.FromArgb(150, 150, 150);
        public static Color AccentPrimary { get; set; } = Color.FromArgb(233, 0, 106);
        public static Color AccentSecondary { get; set; } = Color.FromArgb(138, 0, 106);
        public static Color AccentTertiary { get; set; } = PurplePastel;

        public static Color Highlight { get; set; } = Color.FromArgb(255, 255, 255);

        public static Color Positive { get; set; } = GreenPastel;
        public static Color Negative { get; set; } = RedPastel;

        public static Color Info { get; set; } = BluePastel;
        public static Color Warning { get; set; } = OrangePastel;
        public static Color Error { get; set; } = Negative;

        public static void DrawTitle(params object[] content) => DrawCustomHeader(Highlight, AccentPrimary, 1, 1, 1, 1, content);
        public static void DrawSubtitle(params object[] content) => DrawCustomHeader(Highlight, AccentSecondary, 1, 1, 0, 0, content);

        public static void DrawInfoMessage(params object[] content) => DrawIconWithContent(Info, Color.Transparent, "[ info  ]", content);
        public static void DrawErrorMessage(params object[] content) => DrawIconWithContent(Error, Color.Transparent, "[ error ]", content);
        public static void DrawWarningMessage(params object[] content) => DrawIconWithContent(Warning, Color.Transparent, "[ warn  ]", content);

        public static void DrawInfoMessageLine(params object[] content)
        {
            DrawInfoMessage(content);
            ExtendedConsole.WriteLine();
        }

        public static void DrawErrorMessageLine(params object[] content)
        {
            DrawErrorMessage(content);
            ExtendedConsole.WriteLine();
        }

        public static void DrawWarningMessageLine(params object[] content)
        {
            DrawWarningMessage(content);
            ExtendedConsole.WriteLine();
        }

        private static void DrawIconWithContent(Color foreground, Color background, string icon, params object[] content)
        {
            ExtendedConsole.Write(ExtendedConsole.ConsoleColourArea.Background, background, foreground, icon);
            ExtendedConsole.Write(" ");
            ExtendedConsole.Write(content);
        }

        public static void DrawCustomHeader(Color foreground, Color background, int paddingLeft = 1, int paddingRight = 1, int paddingTop = 1, int paddingBottom = 1, params object[] content)
        {
            var contentList = content.ToList();
            var consoleWidth = Console.WindowWidth;
            var totalTextAreaWidth = contentList.Where(contentItem => contentItem is string).Sum(@string => (@string as string).Length) + paddingLeft + paddingRight;
            var flankingBorderWidths = (consoleWidth - totalTextAreaWidth) / 2d;
            var borderCharacter = ExtendedConsole.HighColourMode ? ' ' : '-';

            DrawFullBorderSection(paddingTop, foreground, background, borderCharacter);
            DrawInlineBorderSection((int)Math.Floor(flankingBorderWidths) + paddingLeft, foreground, background, borderCharacter);

            contentList.InsertRange(0, new List<object> { ExtendedConsole.ConsoleColourArea.Foreground, foreground, ExtendedConsole.ConsoleColourArea.Background, background });

            ExtendedConsole.Write(contentList.ToArray());

            DrawInlineBorderSection((int)Math.Ceiling(flankingBorderWidths) + paddingRight, foreground, background, borderCharacter);

            Console.WriteLine();

            DrawFullBorderSection(paddingBottom, foreground, background, borderCharacter);
        }

        public static void DrawColourMap()
        {
            var element = "".PadRight(5, '#');

            for (var b = 0; b < 255; b += 10)
            {
                for (var g = 0; g < 255; g += 10)
                {
                    for (var r = 0; r < 255; r += 10)
                    {
                        ExtendedConsole.Write(ExtendedConsole.ConsoleColourArea.Background, Color.FromArgb(r, g, b), Color.FromArgb(255 - r, 255 - g, 255 - b), element);
                    }

                    ExtendedConsole.WriteLine();
                }
            }
        }

        private static void DrawFullBorderSection(int thickness, Color foregroundColour, Color backgroundColor, char borderCharacter = ' ')
        {
            var consoleWidth = Console.WindowWidth;

            for (var padding = 0; padding < thickness; ++padding)
            {
                DrawInlineBorderSection(consoleWidth, foregroundColour, backgroundColor, borderCharacter);
            }
        }

        private static void DrawInlineBorderSection(int length, Color foregroundColour, Color backgroundColor, char borderCharacter = ' ')
        {
            var textToPrint = length > 0 ? "".PadRight(length, borderCharacter) : "";

            ExtendedConsole.Write(ExtendedConsole.ConsoleColourArea.Foreground, foregroundColour, ExtendedConsole.ConsoleColourArea.Background, backgroundColor, textToPrint);
        }
    }
}
