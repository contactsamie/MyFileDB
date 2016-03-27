using System.Security.Cryptography.X509Certificates;

namespace MyFileDB.Common.Services
{
    public interface IFileService
    {
        void Update(string fileName, FileContent content);

        void Create(string fileName, FileContent content);

        FileContent Read(string fileName,string bodyType);

        bool Exists(string fileName);

        void CreateDirectoryIfItDoesntExist(string pathName);

        void Delete(string s);
    }
}