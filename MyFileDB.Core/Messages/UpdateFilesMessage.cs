using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class UpdateFilesMessage
    {
        public UpdateFilesMessage(List<UpdateOneFileIdentityMessage> updateOneFileMessages)
        {
            UpdateOneFileMessages = updateOneFileMessages ?? new List<UpdateOneFileIdentityMessage>();
        }

        public List<UpdateOneFileIdentityMessage> UpdateOneFileMessages { private set; get; }
    }
}