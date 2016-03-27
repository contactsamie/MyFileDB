namespace MyFileDB.Core.Messages
{
    public class EachFileDeletedMessage
    {

        public EachFileDeletedMessage(DeleteOneFileIdentityMessage deleteOneFileIdentityMessage)
        {
            DeleteOneFileIdentityMessage = deleteOneFileIdentityMessage;
        }

        public DeleteOneFileIdentityMessage DeleteOneFileIdentityMessage { private set; get; }
    }
}