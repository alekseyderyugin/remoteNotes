using System;

namespace remoteNotesLib
{
    public enum State
    {
        Added,
        Updated,
        Deleted,
        NoChange
    }

    [Serializable]
    public class Note
    {
        public Guid id;
        public State state;

        public string title;
        public string content;

        public Note(string title, string content)
        {
            id = Guid.NewGuid();
            state = State.NoChange;

            this.title = title;
            this.content = content;
        }

        public string Inspect()
        {
            String id = "id: " + this.id;
            String state = "state: " + this.state;
            String title = "title: " + this.title;
            return id + ", " + state + ", " + title;
        }

        public override bool Equals(object other)
        {
            if (other == null) {
                return false;
            } else {
                Note otherNote = other as Note;
                if (otherNote == null) {
                    return false;
                } else {
                    return id == otherNote.id;
                }
            }
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
