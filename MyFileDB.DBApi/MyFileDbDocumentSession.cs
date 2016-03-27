using Akka.Actor;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.DBApi
{
    public class MyFileDbDocumentSession : IMyFileDbDocumentSession
    {
        public MyFileDbDocumentSession(MyFileDbDocumentStore documentStore)
        {
            DocumentStore = documentStore;
            CurrentLoads = new Dictionary<string, FileContentUpdateMessage>();
        }

        public MyFileDbDocumentStore DocumentStore { get; set; }

        public void Delete<T>(string id)
        {
            DocumentStore.MyFileDbActorRef.Tell(new DeleteFilesMessage(new List<DeleteOneFileIdentityMessage>()
            {
                new DeleteOneFileIdentityMessage(null)
                {
                    FileName = id+".json",
                    FolderName = DocumentStore.Store.FolderName,
                    RootPath = DocumentStore.Store.RootPath
                }
            }));
        }

        private Dictionary<string, FileContentUpdateMessage> CurrentLoads { set; get; }

        public object Load(Type type,string id) 
        {

            var result = DocumentStore.MyFileDbActorRef.Ask(new LoadFileContentMessage(DocumentStore.Store.RootPath, DocumentStore.Store.FolderName, id + ".json", null, type.AssemblyQualifiedName)).Result as FileContentUpdateMessage;

            if (CurrentLoads.ContainsKey(id))
            {
                CurrentLoads[id] = result;
            }
            else
            {
                CurrentLoads.Add(id, result);
            }

            
            return result?.FileContent?.Body;
        }

        public T Load<T>(string id) where T : class
        {
            var doc= Load(typeof(T), id) as T;
            return doc;
        }

        public DocumentCreateResult Create<T>(T document, bool withoutWaiting = false, string id=null) where T : class
        {
         
            id =id??  (typeof(T).Name + "-" + Guid.NewGuid() );
            var newEtag = Guid.NewGuid();
            DocumentStore.MyFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileIdentityMessage>()
              {
                  new StoreOneFileIdentityMessage()
                  {
                      FileName =id+ ".json" ,
                      FolderName = DocumentStore.Store.FolderName,
                      RootPath =  DocumentStore.Store.RootPath,
                      FileContent =new FileContent( document,newEtag.ToString(),typeof(T).AssemblyQualifiedName)
                  }
              }));
            if (!withoutWaiting)
            {
                   WaitUnitil(() =>
                   {
                      var doc= Load<T>(id);
                       if (doc != null)
                       {
                           return true;
                       }
                       else
                       {
                           return false;
                       }
                     
                   });
            }
            return new DocumentCreateResult(newEtag, id);
        }

        public void SaveChanges(bool withoutWaiting=false)
        {
            var updates = CurrentLoads.Select(load => new UpdateOneFileIdentityMessage(load.Value.FileContent)
            {
                FileName = load.Value.FileName,
                FolderName = load.Value.FolderName,
                RootPath = load.Value.RootPath
            }).ToList();
            if (updates.Count > 0)
            {
                DocumentStore.MyFileDbActorRef.Tell(new UpdateFilesMessage(updates));
            }

            if (!withoutWaiting)
            {
               WaitUnitil(() => updates.All(x => Load(Type.GetType(x.FileContent.DataType), x.FileName.Replace(".json", "")) != null));
            }
        }

        public IEnumerable<T> Query<T>(Func<IQueryable<T>, IQueryable<T>> queryFunction=null) where T : class
        {
            var files = DocumentStore.MyFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(DocumentStore.Store.RootPath, DocumentStore.Store.FolderName)).Result as IEnumerable<FileContentUpdateMessage>;
            var queryable = (files ?? new List<FileContentUpdateMessage>()).Select(x => x.FileContent.Body as T).AsQueryable();
            //todo - not safe by default
            return queryFunction!=null?queryFunction(queryable):queryable;
        }
        public void WaitUnitil(Func<bool> assert)
        {
            const int maxCount = 5000;
            var counter = 0;
            while (counter < maxCount)
            {
                System.Threading.Thread.Sleep(10 * counter);
               
                    if (assert())
                    {
                        return;
                    }
                counter++;
            }
            throw new Exception("Unable to determine if operation was successfull" );
        }
        public void Dispose()
        {
            // session dispose should leave store alone
         // DocumentStore.Dispose();
        }
    }
}