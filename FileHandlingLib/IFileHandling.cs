namespace Librac.FileHandlingLib
{
    internal interface IFileHandling
    {
        void ApplyReadOnlyToDirectory(string directoryPath);
        void RemoveReadOnlyFromDirectory(string directoryPath);
        void CopyFileTo(string fileFullName, string destinationFullName, bool overwrite);
        void MoveFileTo(string fileFullName, string destinationFullName, bool overwrite);
    }
}