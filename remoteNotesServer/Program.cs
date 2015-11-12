using System;
using System.Runtime.Remoting;

namespace remoteLibServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            RemotingConfiguration.Configure("remoteLibServer.exe.config", false);

            Console.WriteLine("Server started. Press Enter to end");
            Console.ReadLine();
        }
    }
}
