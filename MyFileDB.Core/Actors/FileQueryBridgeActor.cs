using Akka.Actor;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;

namespace MyFileDB.Core.Actors
{
    public class FileQueryBridgeActor : ReceiveActor
    {
        public FileQueryBridgeActor(IFileService fileService)
        {
            Receive<LoadFileContentMessage>(message =>
            {
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                var fileNme = directory + message.FileName;
                if (fileService.Exists(fileNme))
                {
                    var content = fileService.Read(fileNme);
                    Sender.Tell(new FileContentUpdateMessage(fileNme, content, message.CallBackActorRef));
                }
                else
                {
                    Sender.Tell(new FileContentDeleteMessage(fileNme,  message.CallBackActorRef));
                }


            });
        }
    }
}