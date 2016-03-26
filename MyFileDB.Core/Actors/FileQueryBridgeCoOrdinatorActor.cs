using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using MyFileDB.ActorSystemLib;

namespace MyFileDB.Core.Actors
{
    public class FileQueryBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileQueryBridgeRef { set; get; }

        private Dictionary<string, FileContentMessage> FileContentMessages { set; get; }

        public FileQueryBridgeCoOrdinatorActor()
        {
            FileContentMessages = new Dictionary<string, FileContentMessage>();

            FileQueryBridgeRef = Context.System.ActorOf(Context.System.DI().Props<FileQueryBridgeActor>().WithRouter(new RoundRobinPool(5)));

            Receive<LoadFileContentMessages>(message =>
            {
                foreach (var fileContentMessage in message.FileContentMessages)
                {
                    FileQueryBridgeRef.Tell(fileContentMessage);
                }
            });

            Receive<FileContentMessage>(message =>
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
        }

        // e.g. Restart the child, if 10 exceptions occur in 30 seconds or less, then stop the actor
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x => Directive.Restart);
        }
    }
}