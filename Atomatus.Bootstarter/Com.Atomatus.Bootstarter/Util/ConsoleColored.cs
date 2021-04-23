using System;

namespace Com.Atomatus.Bootstarter
{
    internal static class ConsoleColored
    {
        internal static void Write(string text, ConsoleColor? fgColor = null)
        {
            var curr = Console.ForegroundColor;
            Console.ForegroundColor = fgColor ?? curr;
            Console.Write(text);
            Console.ForegroundColor = curr;
        }

        internal static void WriteLine(string text, ConsoleColor? fgColor = null)
        {
            var curr = Console.ForegroundColor;
            Console.ForegroundColor = fgColor ?? curr;
            Console.WriteLine(text);
            Console.ForegroundColor = curr;
        }
    }
}
