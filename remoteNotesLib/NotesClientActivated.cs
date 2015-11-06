using System;
using System.Collections.Generic;

namespace remoteNotesLib {
    public class NotesClientActivated : MarshalByRefObject {
        private List<Note> notes;
        public NotesClientActivated() {
            this.notes = new List<Note>();
            Console.WriteLine("NotesClientActivated was created");
        }
        public void createRecord(Note note){
            note.StateField = StateField.Added;
            this.notes.Add(note);
        }
        public void updateRecord(Note note){
            note.StateField = StateField.Updated;
            int index = notes.IndexOf(note);
            this.notes[index] = note;
        }
        public void deleteRecord(Note note){
            note.StateField = StateField.Deleted;
            int index = notes.IndexOf(note);
            this.notes.RemoveAt(index);
        }
        public List<Note> requestCacheRecords(){
            return this.notes;
        }
        public void clear(){
            this.clear();
        }

    }
}

