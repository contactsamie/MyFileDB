using Akka.Actor;

namespace MyFileDB.Core.Messages
{
    public class FileContentDeleteMessage
    {
        public FileContentDeleteMessage(string fileName,  IActorRef callBackActorRef)
        {
            FileName = fileName;
            CallBackActorRef = callBackActorRef;
        }

        public IActorRef CallBackActorRef { private set; get; }
        public string FileName { private set; get; }
    }
}