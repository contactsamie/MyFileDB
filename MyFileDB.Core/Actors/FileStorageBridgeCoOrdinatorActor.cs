using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Messages;
using System;

namespace MyFileDB.Core.Actors
{
    public class FileStorageBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileStorageBridgeActorRef { set; get; }

        public FileStorageBridgeCoOrdinatorActor()
        {
            FileStorageBridgeActorRef = Context.System.ActorOf(Context.System.DI().Props<FileStorageBridgeActor>().WithRouter(SystemActor.CommonRouterConfig));

            Receive<StoreFilesMessage>(message =>
            {
                foreach (var storeOneFileMessage in message.StoreOneFileMessages)
                {
                    FileStorageBridgeActorRef.Forward(storeOneFileMessage);
                }

                Sender.Tell(new FilesStoredMessage());
            });
        }

        // if any child, e.g. the FileStorageBridgeActorRef above. throws an exception
        // apply the rules below
        // e.g. Restart the child, if 10 exceptions occur in 30 seconds or
        // less, then stop the actor
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x => Directive.Restart);
        }
    }
}