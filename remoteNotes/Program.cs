using System;
using System.Runtime.Remoting;
using remoteNotesLib;
using Gtk;

namespace remoteNotes
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(1000);

            RemotingConfiguration.Configure("remoteNotes.exe.config", false);

            NotesSingleton singleton = new NotesSingleton();

            Console.WriteLine(singleton.getPesistentData().Count);

            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();

            Console.ReadLine();
        }
    }
}
