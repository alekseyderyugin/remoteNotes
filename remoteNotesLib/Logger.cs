using System;

namespace remoteNotesLib
{
    public class Logger
    {
        public static void Write(String s)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "| " + s);
        }
    }
}
