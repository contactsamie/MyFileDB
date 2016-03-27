using System.Collections.Generic;
using Akka.Actor;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;


namespace MyFileDB.Core.Actors
{
    public class FileStorageBridgeActor : ReceiveActor
    {
        public FileStorageBridgeActor(IFileService fileService)
        {
            Receive<StoreOneFileIdentityMessage>(message =>
            {
                //todo store file
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                fileService.Write(directory + message.FileName, message.FileContent);
                Sender.Tell(new EachFileStoredMessage(message));
                
                //tell to update file cache
              ApplicationActorSystem.ActorReferences.ApplicationActorRef.Tell(new LoadAllFileContentMessage(new List<LoadFileContentMessage>()
              {
                  new LoadFileContentMessage(message.RootPath,message.FolderName,message.FileName,Sender)
              }));
            });
        }
    }
}