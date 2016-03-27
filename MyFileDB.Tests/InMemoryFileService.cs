using MyFileDB.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyFileDB.Tests
{
    public class InMemoryFileService : IFileService
    {
        public static Dictionary<string, string> InMemoryFileStorage = new Dictionary<string, string>();

        public void Write(string fileName, string content)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            InMemoryFileStorage = InMemoryFileStorage ?? new Dictionary<string, string>();
            if (!Exists(fileName))
            {
                InMemoryFileStorage.Add(fileName, content);
            }
            else
            {
                InMemoryFileStorage[fileName] = content;
            }
        }

        public string Read(string fileName)
        {
            if (Exists(fileName))
            {
                return InMemoryFileStorage[fileName];
            }
            throw new FileNotFoundException();
        }

        public bool Exists(string fileName)
        {
            return InMemoryFileStorage.ContainsKey(fileName);
        }

        public void CreateDirectoryIfItDoesntExist(string pathName)
        {
        }

        public void Delete(string fileName)
        {
            if (Exists(fileName))
            {
                 InMemoryFileStorage.Remove(fileName);
            }
        }
    }
}