namespace MyFileDB.Core.Messages
{
    public class DeleteOneFileIdentityMessage: AFileIdentityMessage
    {
        public DeleteOneFileIdentityMessage(string fileContentBodyType)
        {
            FileContentBodyType = fileContentBodyType;
        }

        public string FileContentBodyType { get; private set; }
    }
}