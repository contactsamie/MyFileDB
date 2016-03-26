using Akka.Actor;

namespace MyFileDB.Core.Messages
{
    public class FileContentMessage
    {
        public FileContentMessage(string fileName, string fileContent, IActorRef callBackActorRef)
        {
            FileName = fileName;
            FileContent = fileContent;
            CallBackActorRef = callBackActorRef;
        }

        public IActorRef CallBackActorRef { private set; get; }
        public string FileName { private set; get; }
        public string FileContent { get; private set; }
    }
}