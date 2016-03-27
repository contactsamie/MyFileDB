namespace MyFileDB.Core.Messages
{
    public abstract  class AFileIdentityMessage
    {
      public  string RootPath { set; get; }
      public  string FolderName { set; get; }
      public  string FileName { set; get; }
     
    }
}