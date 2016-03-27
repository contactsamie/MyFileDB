using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.DBApi
{
    public interface IMyFileDbDocumentSession:IDisposable
    {
        MyFileDbDocumentStore DocumentStore { get; }

        void Delete<T>(string id);

        T Load<T>(string id) where T : class;

        object Load(Type type, string id);

        DocumentCreateResult Create<T>(T document,bool withoutWaiting=false,string id=null) where T : class;

        

        void SaveChanges(bool withoutWaiting = false);
        //todo - not safe by default
        IEnumerable<T> Query<T>(Func<IQueryable<T>, IQueryable<T>> queryFunction=null ) where T : class;
    }
}