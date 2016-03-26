using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class LoadFileContentMessages
    {
        public LoadFileContentMessages(List<LoadFileContentMessage> fileContentMessages)
        {
            FileContentMessages = fileContentMessages;
        }

        public List<LoadFileContentMessage> FileContentMessages { get; private set; }
    }
}