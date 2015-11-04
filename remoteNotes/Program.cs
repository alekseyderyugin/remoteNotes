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
						HttpClientChannel channel = new HttpClientChannel();
						ChannelServices.RegisterChannel(channel);

						// Go get the remote object
						object notesSingleton = Activator.GetObject(
								typeof(remoteNotesLib.NotesSingleton),
								"http://localhost:13101/notesSingleton.soap"
						);

						//object noteTransactionSinglecall = Activator.GetObject(
								//typeof(remoteNotesLib.NoteTransactionSinglecall),
								//"http://localhost:13000/NoteTransactionSinglecall.soap"
						//);

						//object notesClientActivated = Activator.GetObject(
								//typeof(remoteNotesLib.NotesClientActivated),
								//"http://localhost:13000/NotesClientActivated.soap"
						//);

						// Cast the returned proxy to the SimpleMath type
						NotesSingleton singleton = (NotesSingleton)notesSingleton;
						//NoteTransactionSinglecall singlecall = (NoteTransactionSinglecall)notesSingleton;
						//NotesClientActivated clientActivated = (NotesClientActivated)notesClientActivated;

						// Use the remote object
						singleton.getPersistentData();	

						Application.Init();
						MainWindow win = new MainWindow();
						win.Show();
						Application.Run();

						Console.ReadLine();
				}
		}
}
