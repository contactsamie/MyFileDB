using MyFileDB.Common.Services;

namespace MyFileDB.Core.Messages
{
    public class UpdateOneFileIdentityMessage : AFileIdentityMessage
    {
        public UpdateOneFileIdentityMessage(FileContent fileContent)
        {
            FileContent = fileContent;
        }

        public FileContent FileContent { get; private set; }

    }
}