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

        private Guid id;
        public StateField StateField;

        public Note()
        {
            title = "";
            content = "";

            id = Guid.NewGuid();
            StateField = StateField.NoChange;
        }

        public Note(string title, string content)
        {
            this.title = title;
            this.content = content;

            id = Guid.NewGuid();
        }
    }
}
