using System;

namespace InDoOut_Console.Display
{
    internal static class ColourConsole
    {
        public static void Write<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class => WrapColour(() => Console.Write(value), colour);
        public static void Write(ColourBlock block) => Write(block.Value, block.Colour);
        public static void Write(params ColourBlock[] blocks)
        {
            foreach (var block in blocks)
            {
                Write(block);
            }
        }

        public static void WriteLine<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class => WrapColour(() => Console.WriteLine(value), colour);
        public static void WriteLine(params ColourBlock[] blocks)
        {
            Write(blocks);
            Console.WriteLine();
        }

        public static void WriteError<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteErrorIcon();
            Write(value, colour);
        }

        public static void WriteErrorLine<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteError(value, colour);
            Console.WriteLine();
        }

        public static void WriteErrorLine(params ColourBlock[] blocks)
        {
            WriteErrorIcon();
            WriteLine(blocks);
        }

        public static void WriteWarning<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteWarningIcon();
            Write(value, colour);
        }

        public static void WriteWarningLine<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteWarning(value, colour);
            Console.WriteLine();
        }

        public static void WriteWarningLine(params ColourBlock[] blocks)
        {
            WriteWarningIcon();
            WriteLine(blocks);
        }

        public static void WriteInfo<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteInfoIcon();
            Write(value, colour);
        }

        public static void WriteInfoLine<T>(T value, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            WriteInfo(value, colour);
            Console.WriteLine();
        }

        public static void WriteInfoLine(params ColourBlock[] blocks)
        {
            WriteInfoIcon();
            WriteLine(blocks);
        }

        private static void WriteErrorIcon(ConsoleColor colour = ConsoleColor.Red) => WriteIcon("!", colour);
        private static void WriteWarningIcon(ConsoleColor colour = ConsoleColor.Yellow) => WriteIcon("-", colour);
        private static void WriteInfoIcon(ConsoleColor colour = ConsoleColor.Cyan) => WriteIcon("i", colour);

        private static void WriteIcon<T>(T iconContent, ConsoleColor colour = ConsoleColor.White) where T : class
        {
            Write("[", colour);
            Write(iconContent, colour);
            Write("] ", colour);
        }

        private static void WrapColour(Action action, ConsoleColor colour = ConsoleColor.White)
        {
            var oldColour = Console.ForegroundColor;

            Console.ForegroundColor = colour;
            action?.Invoke();
            Console.ForegroundColor = oldColour;
        }
    }
}
