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
				public string title;
				public string content;
				public StateField StateField;

				private int id;
				private DateTime date;

				public Note()
				{
						id = base.GetHashCode();
						StateField = StateField.NoChange;
						date = DateTime.Now;
						title = "";
						content = "";					
								
				}
				
				public Note(string title, string content) {
						this.title = title;
						this.content = content;
						id = base.GetHashCode();
						date = DateTime.Now;
				}
		}
}
