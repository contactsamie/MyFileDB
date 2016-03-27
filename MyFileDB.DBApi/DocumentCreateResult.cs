using System;

namespace MyFileDB.DBApi
{
    public class DocumentCreateResult
    {
        public DocumentCreateResult(Guid? eTag, string id)
        {
            ETag = eTag;
            Id = id;
        }

        public Guid? ETag { private set; get; }
        public string Id { private set; get; }
    }
}