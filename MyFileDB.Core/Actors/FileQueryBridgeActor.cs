using Akka.Actor;
using MyFileDB.Core.Messages;
using MyFileDB.Core.Services;

namespace MyFileDB.Core.Actors
{
    public class FileQueryBridgeActor : ReceiveActor
    {
        public FileQueryBridgeActor(IFileService fileService)
        {
            Receive<StoreOneFileMessage>(message =>
            {
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                var fileNme = directory + message.FileName;
                var content = fileService.Read(fileNme);
                Sender.Tell(new FileContentMessage(fileNme, content));
            });
        }
    }
}