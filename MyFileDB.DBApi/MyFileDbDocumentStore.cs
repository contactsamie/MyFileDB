using Akka.Actor;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core;
using MyFileDB.DependencyLib;
using System;
using Akka.Routing;
using Autofac;

namespace MyFileDB.DBApi
{
    public class MyFileDbDocumentStore : IDisposable
    {
        internal IActorRef MyFileDbActorRef { set; get; }

        internal IFleStore Store { set; get; }

        internal IMyFileDbDocumentSession Session { set; get; }

        public IMyFileDbDocumentSession OpenSession()
        {
            Session = new MyFileDbDocumentSession(this);
            MyFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            if (Session == null)
            {
                throw new Exception("Session not set");
            }
            return Session;
        }

        private const string RootDbName = "MyFileDB";

        public MyFileDbDocumentStore(IFleStore store,IContainer container, RouterConfig routerConfig=null)
        {
            //todo allow passing of routerConfig
            if (store == null) throw new ArgumentNullException(nameof(store));
            store.RootPath = (store.RootPath ?? "") + "\\" + RootDbName;
            store.FolderName = store.FolderName ?? "";
            Store = store;

      
            if (ApplicationActorSystem.ActorReferences.ApplicationActorRef != null) return;
            ApplicationActorSystem.Create<SystemActor>(container);
           

            
        }

        public void Dispose()
        {
            //document store dispose should not shut down akka
           // ApplicationActorSystem.ShutDown();
        }
    }
}