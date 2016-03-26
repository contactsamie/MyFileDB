using MyFileDB.Core.Services;

namespace MyFileDB.Tests
{
    public class FakeFileService : IFileService
    {
        public void Write(string fileName, string content)
        {
           
        }

        public string Read(string fileName)
        {
            return "";
        }

        public bool Exists(string fileName)
        {
            return true;
        }

        public void CreateDirectoryIfItDoesntExist(string pathName)
        {
           
        }
    }
}