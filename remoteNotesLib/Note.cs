using System;


namespace remoteNotesLib
{
		public enum StateField
		{
				Added,
				Updated,
				Deleted,
				NoChange
		}
		
		public class Note
		{
				private int id;
				private DateTime date;
				public string title;
				public string content;
				private StateField StateField;

				public Note()
				{
								this.id = base.GetHashCode();
								this.StateField = StateField.NoChange;
								this.date = DateTime.Now;

				}
		}
}

