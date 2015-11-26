using System;
using System.Runtime.Remoting;
using Gtk;

namespace remoteNotes
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Если сервер и клиент запускаются из ИДЕ (по порядку, но практически одновременно),
            //сервер не успевает создать сокет, поэтому надо немного подождать
            System.Threading.Thread.Sleep(1000);

            RemotingConfiguration.Configure("remoteNotes.exe.config", false);

            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();

            Console.ReadLine();
        }
    }
}
