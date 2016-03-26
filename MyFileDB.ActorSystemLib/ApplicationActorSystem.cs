using System;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;

namespace MyFileDB.ActorSystemLib
{
    public static  class ApplicationActorSystem
    {
        public static ActorSystem ActorSystem { set; get; }

        public static IContainer Container { set; get; }

        public static void Create<T>(IContainer container, ActorSystem actorSystem=null) where T : ActorBase
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).Where(t => typeof(ReceiveActor).IsAssignableFrom(t));
            builder.Update(container);
            Container = container;
            ActorSystem =  actorSystem?? Akka.Actor.ActorSystem.Create(typeof(ApplicationActorSystem).Name);
            IDependencyResolver resolver = new AutoFacDependencyResolver(container, ActorSystem);
            ActorReferences.ApplicationActorRef = ActorSystem.ActorOf(ActorSystem.DI().Props<T>(),typeof(T).Name);
        }

        public static async void ShutDown()
        {
            await  ActorSystem.Terminate();
        }

        public static class ActorReferences
        {
            public static IActorRef ApplicationActorRef { get; set; }
        }
    }
}