using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Messages;

namespace MyFileDB.Core.Actors
{
    public class FileDeleteBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileDeleteBridgeActorRef { set; get; }

        public FileDeleteBridgeCoOrdinatorActor()
        {
            FileDeleteBridgeActorRef = Context.System.ActorOf(Context.System.DI().Props<FileDeleteBridgeActor>().WithRouter(new RoundRobinPool(5)));

            Receive<DeleteFilesMessage>(message =>
            {
                foreach (var deleteOneFileMessage in message.DeleteOneFileMessages)
                {
                    FileDeleteBridgeActorRef.Forward(deleteOneFileMessage);
                }

                Sender.Tell(new FilesDeletedMessage());
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