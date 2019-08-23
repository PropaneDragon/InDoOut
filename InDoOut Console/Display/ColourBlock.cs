using System;

namespace InDoOut_Console.Display
{
    internal class ColourBlock
    {
        public object Value { get; private set; } = null;
        public ConsoleColor Colour { get; private set; } = ConsoleColor.White;

        public ColourBlock(object value, ConsoleColor colour = ConsoleColor.White)
        {
            Value = value;
            Colour = colour;
        }
    }
}
