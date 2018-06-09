using System;

namespace PerformanceTests.Infrastructure.Logging
{
    public class ConsoleOutputUtils
    {
        public static void WriteLine(
            ConsoleColor foregroundColor,
            ConsoleColor backgroundColor,
            string messageFromat,
            params object[] args)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(string.Format(messageFromat, args));
        }

        public static void DebugFormat(string format, params object[] args)
        {
            WriteLine(ConsoleColor.White, ConsoleColor.Black, "Debug => " + format, args);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            WriteLine(ConsoleColor.Green, ConsoleColor.Black, "Info => " + format, args);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            WriteLine(ConsoleColor.Yellow, ConsoleColor.Black, "Warn => " + format, args);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            WriteLine(ConsoleColor.Red, ConsoleColor.Black, "Error => " + format, args);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            WriteLine(ConsoleColor.DarkRed, ConsoleColor.White, "Fatal => " + format, args);
        }
    }
}
