namespace MyFileDB.Common.Services
{
    public interface IFileService
    {
        void Write(string fileName, string content);

        string Read(string fileName);

        bool Exists(string fileName);

        void CreateDirectoryIfItDoesntExist(string pathName);
    }
}