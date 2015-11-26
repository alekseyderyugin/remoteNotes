using System;

namespace remoteNotesLib
{
    public class NotesTransactionSinglecall : MarshalByRefObject
    {
        public NotesTransactionSinglecall()
        {
            Logger.Write("NoteTransactionSinglecall was created");
        }

        public void Commit(NotesClientActivated clientCache)
        {

        }

        public void Rollback(NotesClientActivated clientCache)
        {
            //clientCache.clear();
        }
    }
}
