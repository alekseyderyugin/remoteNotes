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
		[Serializable]
		public class Note
		{
				private int id;
				private DateTime date;
				public string title;
				public string content;
				public StateField StateField;

				public Note()
				{
						this.id = base.GetHashCode();
						this.StateField = StateField.NoChange;
						this.date = DateTime.Now;
						this.title = "";
						this.content = "";					
						
				}
			public Note(string title, string content){
						this.title = title;
						this.content = content;
						this.id = base.GetHashCode();
						this.date = DateTime.Now;
			}
}

