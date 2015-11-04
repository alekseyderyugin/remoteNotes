using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using remoteNotesLib;
using Gtk;

namespace remoteNotes
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();
			MainWindow win = new MainWindow();
			win.Show();
			Application.Run();

			HttpClientChannel channel = new HttpClientChannel();
			ChannelServices.RegisterChannel(channel);

			// Go get the remote object
			object remoteObj = Activator.GetObject(
				typeof(remoteNotesLib.MyClass),
				"http://localhost:13101/myURI.soap"
			);

			// Cast the returned proxy to the SimpleMath type
			MyClass obj = (MyClass)remoteObj;

			// Use the remote object
			obj.getMessage("hui na na!");

			// Ask user to press enter
			Console.Write("Press enter to end");
			Console.ReadLine();
		}
	}
}
