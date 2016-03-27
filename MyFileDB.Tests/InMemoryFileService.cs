using MyFileDB.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyFileDB.Tests
{
    public class InMemoryFileService : IFileService
    {
        public InMemoryFileService()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new NonPublicPropertiesResolver()
            };
        }
        public static Dictionary<string, string> InMemoryFileStorage = new Dictionary<string, string>();

        public void Update(string fileName, FileContent fileContent)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            var old = Read(fileName,fileContent.DataType);
            if (old.Etag != fileContent.Etag)
            {
                throw new Exception("Document have changed since last read");
            }
           
            var content = JsonConvert.SerializeObject(new FileContent(fileContent.Body, Guid.NewGuid().ToString(), fileContent.DataType));
            InMemoryFileStorage[fileName] = content;
        }

        public void Create(string fileName, FileContent fileContent)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            InMemoryFileStorage = InMemoryFileStorage ?? new Dictionary<string, string>();
            if (Exists(fileName)) throw new Exception("Document already exists");
            var content = JsonConvert.SerializeObject(new FileContent(fileContent.Body, Guid.NewGuid().ToString(), fileContent.DataType));
          
             InMemoryFileStorage.Add(fileName, content);
         
        }

        public FileContent Read(string fileName,string bodyType)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!Exists(fileName)) throw new FileNotFoundException();

            var content= InMemoryFileStorage[fileName];
            var tmpContent= JsonConvert.DeserializeObject<FileContent>(content);

            if (string.IsNullOrEmpty(bodyType) ||  tmpContent.Body==null) return tmpContent;
            var jObject  = tmpContent.Body as JObject;
            if (jObject == null)
            {
                return tmpContent;
            }

            var obj = jObject.ToObject(Type.GetType(bodyType));
            return new FileContent(obj, tmpContent.Etag, tmpContent.DataType);
        }

        public bool Exists(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            return InMemoryFileStorage.ContainsKey(fileName);
        }

        public void CreateDirectoryIfItDoesntExist(string pathName)
        {
        }

        public void Delete(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (Exists(fileName))
            {
                 InMemoryFileStorage.Remove(fileName);
            }
        }
    }
}