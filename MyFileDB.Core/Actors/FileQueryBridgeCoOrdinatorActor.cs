using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.Core.Actors
{
    public class FileQueryBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileQueryBridgeRef { set; get; }

        private Dictionary<string, FileContentUpdateMessage> FileContentMessages { set; get; }

        public FileQueryBridgeCoOrdinatorActor()
        {
            FileContentMessages = new Dictionary<string, FileContentUpdateMessage>();

            FileQueryBridgeRef = Context.System.ActorOf(Context.System.DI().Props<FileQueryBridgeActor>().WithRouter(SystemActor.CommonRouterConfig));

            Receive<LoadAllFileContentMessage>(message =>
            {
                foreach (var fileContentMessage in message.FileContentMessages)
                {
                    FileQueryBridgeRef.Tell(fileContentMessage);
                }
            });

            //update cache and call back
            Receive<FileContentUpdateMessage>(message =>
            {
                if (FileContentMessages.ContainsKey(message.FileName))
                {
                    FileContentMessages[message.FileName] = message;
                }
                else
                {
                    FileContentMessages.Add(message.FileName, message);
                }
                if (message.CallBackActorRef != null)
                {
                    message.CallBackActorRef.Tell(new LoadFileContentsResultMessages(FileContentMessages.Select(x => x.Value).ToList()));
                }
            });

            Receive<FileContentDeleteMessage>(message =>
            {
                if (FileContentMessages.ContainsKey(message.FileName))
                {
                    FileContentMessages.Remove(message.FileName);
                }
                if (message.CallBackActorRef != null)
                {
                    message.CallBackActorRef.Tell(new LoadFileContentsResultMessages(FileContentMessages.Select(x => x.Value).ToList()));
                }
            });

            Receive<LoadFileContentMessage>(message =>
            {
                FileContentUpdateMessage file;
                if (FileContentMessages.ContainsKey(message.FileName))
                {
                    file = FileContentMessages[message.FileName];
                }
                else
                {
                    file = new FileContentUpdateMessage(message.FileName, new FileContent(null, null, message.FileContentBodyType), message.CallBackActorRef, message.FolderName, message.RootPath);
                }

                Sender.Tell(file);
            });
            Receive<ListAllFilesByFolderNameMessage>(message =>
            {
                Sender.Tell(FileContentMessages.Select(x => x.Value).ToList());
            });
        }

        // e.g. Restart the child, if 10 exceptions occur in 30 seconds or less, then stop the actor
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x => Directive.Restart);
        }
    }
}