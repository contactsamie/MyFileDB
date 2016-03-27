namespace MyFileDB.Core.Messages
{
    public class EachFileUpdatedMessage
    {

        public EachFileUpdatedMessage(UpdateOneFileIdentityMessage updateOneFileIdentityMessage)
        {
            UpdateOneFileIdentityMessage = updateOneFileIdentityMessage;
        }

        public UpdateOneFileIdentityMessage UpdateOneFileIdentityMessage { private set; get; }
    }
}