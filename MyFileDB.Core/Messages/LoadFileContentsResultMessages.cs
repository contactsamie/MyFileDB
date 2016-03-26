using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class LoadFileContentsResultMessages
    {
        public LoadFileContentsResultMessages(List<FileContentUpdateMessage> fileContentMessages)
        {
            FileContentMessages = fileContentMessages;
        }

        public List<FileContentUpdateMessage> FileContentMessages { get; private set; }
    }
}