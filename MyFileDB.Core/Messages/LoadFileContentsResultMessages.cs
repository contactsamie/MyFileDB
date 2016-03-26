using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class LoadFileContentsResultMessages
    {
        public LoadFileContentsResultMessages(List<FileContentMessage> fileContentMessages)
        {
            FileContentMessages = fileContentMessages;
        }

        public List<FileContentMessage> FileContentMessages { get; private set; }
    }
}