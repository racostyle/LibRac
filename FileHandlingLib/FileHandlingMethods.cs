using System;
using System.IO;

namespace Librac.FileHandlingLib
{
    internal class FileHandlingMethods : IFileHandling
    {
        public void RemoveReadOnlyFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            try
            {
                // Remove the read-only attribute from all files in the directory
                foreach (string filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
                {
                    var file = new FileInfo(filePath);
                    // Check if the file is read-only and remove the attribute
                    if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }
                Console.WriteLine("Read-only attribute removed from all files successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void ApplyReadOnlyToDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            try
            {
                // Set the read-only attribute to all files in the directory
                foreach (string filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
                {
                    var file = new FileInfo(filePath);
                    file.Attributes |= FileAttributes.ReadOnly;
                }
                Console.WriteLine("Read-only attribute applied to all files successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void MoveFileTo(string fileFullName, string destinationFullName, bool overwrite)
        {
            if (!File.Exists(fileFullName))
                return;
            if (overwrite)
            {
                if (!File.Exists(destinationFullName))
                    File.Delete(destinationFullName);
            }
            try
            {
                File.Move(fileFullName, destinationFullName);
                Console.WriteLine("File copied successfully.");
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }

        public void CopyFileTo(string fileFullName, string destinationFullName, bool overwrite)
        {
            if (!File.Exists(fileFullName))
                return;
            try
            {
                File.Copy(fileFullName, destinationFullName, overwrite);
                Console.WriteLine("File copied successfully.");
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }
    }
}
