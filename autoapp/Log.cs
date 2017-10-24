using System;

namespace AutoApp
{
    class Log
    {
        public static void L(string log, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void L(string log)
        {
            L(log, ConsoleColor.White);
        }
    }
}
