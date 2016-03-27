using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class DeleteFilesMessage
    {
        public DeleteFilesMessage(List<DeleteOneFileIdentityMessage> deleteOneFileMessages)
        {
            DeleteOneFileMessages = deleteOneFileMessages ?? new List<DeleteOneFileIdentityMessage>();
        }

        public List<DeleteOneFileIdentityMessage> DeleteOneFileMessages { private set; get; }
    }
}