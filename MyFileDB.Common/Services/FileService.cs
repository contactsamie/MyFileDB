namespace MyFileDB.Common.Services
{
    public class FileService:IFileService
    {
        public void Write(string fileName, string content)
        {
            System.IO.File.WriteAllText(fileName, content);
        }

        public string Read(string fileName)
        {
            return System.IO.File.ReadAllText(fileName);
        }

        public bool Exists(string fileName)
        {
            return System.IO.File.Exists(fileName);
        }

        public void CreateDirectoryIfItDoesntExist(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }

        public void Delete(string s)
        {
            System.IO.File.Delete(s);
        }
    }
}