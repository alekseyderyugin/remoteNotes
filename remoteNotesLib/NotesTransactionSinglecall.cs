using System;

namespace remoteNotesLib
{
    public class NotesTransactionSinglecall : MarshalByRefObject
    {
        public NotesTransactionSinglecall()
        {
            Logger.Write("NoteTransactionSinglecall was created");
        }

        public void Commit(NotesClientActivated clientActivated)
        {

        }

        public void Rollback(NotesClientActivated clientActivated)
        {
            //clientCache.clear();
        }
    }
}
