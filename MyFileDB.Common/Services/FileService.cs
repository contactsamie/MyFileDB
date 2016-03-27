using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace MyFileDB.Common.Services
{
    public class FileService : IFileService
    {
        public FileService()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new NonPublicPropertiesResolver()
            };
        }
        public void Update(string fileName, FileContent fileContent)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            var old = Read(fileName,fileContent.DataType);
            if (old.Etag != fileContent.Etag)
            {
                throw new Exception("Document have changed since last read");
            }
           // fileContent.Etag = Guid.NewGuid().ToString();
            var content = JsonConvert.SerializeObject(new FileContent(fileContent.Body, Guid.NewGuid().ToString(), fileContent.DataType));
            System.IO.File.WriteAllText(fileName, content);
        }

        public void Create(string fileName, FileContent fileContent)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            if (Exists(fileName)) throw new Exception("Document already exists");
           // fileContent.Etag = Guid.NewGuid().ToString();
            var content = JsonConvert.SerializeObject(new FileContent(fileContent.Body, Guid.NewGuid().ToString(), fileContent.DataType));
            System.IO.File.WriteAllText(fileName, content);
        }

        public FileContent Read(string fileName, string bodyType)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!Exists(fileName)) throw new FileNotFoundException();

            var content = System.IO.File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<FileContent>(content);
        }

        public bool Exists(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            return System.IO.File.Exists(fileName);
        }

        public void CreateDirectoryIfItDoesntExist(string directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }

        public void Delete(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            // if (!Exists(fileName)) throw new Exception("Document does not exists");
            System.IO.File.Delete(fileName);
        }
    }

    public class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            var pi = member as PropertyInfo;
            if (pi != null)
            {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }
            return prop;
        }
    }
}