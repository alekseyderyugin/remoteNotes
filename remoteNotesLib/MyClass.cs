using System;

namespace remoteNotesLib
{
	public class MyClass : MarshalByRefObject
	{
		public MyClass()
		{
			Console.WriteLine("Contructor called");
		}

		public void getMessage(string arg)
		{
			Console.WriteLine(arg);
		}
	}
}
