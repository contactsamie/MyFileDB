using Akka.Actor;
using MyFileDB.Core.Messages;
using MyFileDB.Core.Services;

namespace MyFileDB.Core.Actors
{
    public class FileStorageBridgeActor : ReceiveActor
    {
        public FileStorageBridgeActor(IFileService fileService)
        {
            Receive<StoreOneFileMessage>(message =>
            {
                //todo store file
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                fileService.Write(directory + message.FileName, message.FileContent);
                Sender.Tell(new EachFileStoredMessage());
            });
        }
    }
}