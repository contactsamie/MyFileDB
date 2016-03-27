namespace MyFileDB.Common.Services
{
    public class FileContent
    {
        public FileContent(object body, string etag, string dataType)
        {
            Body = body;
            Etag = etag;
            DataType = dataType;
        }

        public object Body { private set; get; }
        public string Etag { private set; get; }
        public string DataType { private set; get; }
    }
}