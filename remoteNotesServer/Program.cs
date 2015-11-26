using System;
using System.Runtime.Remoting;
using remoteNotesLib;

namespace remoteLibServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            RemotingConfiguration.Configure("remoteLibServer.exe.config", false);

            Logger.Write("Server started. Press any key to shutdown");
            Console.ReadLine();
        }
    }
}
