using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class LoadAllFileContentMessage
    {
        public LoadAllFileContentMessage(List<LoadFileContentMessage> fileContentMessages)
        {
            FileContentMessages = fileContentMessages;
        }

        public List<LoadFileContentMessage> FileContentMessages { get; private set; }
    }
}