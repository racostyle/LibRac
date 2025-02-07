using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Librac.FileHandlingLib
{
    internal class FileHandlerMethods
    {
        #region READONLY METHODS
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
        #endregion

        #region MODE AND COPY
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

        #endregion

        #region SEARCH FOR FILE AND COPY INTO WORKING DIRECTORY
        internal string GetAssemblyDirectory(string currentWorkingDirectory, string assemblyName)
        {
            var index = currentWorkingDirectory.IndexOf(assemblyName) + assemblyName.Length;
            return currentWorkingDirectory.Substring(0, index);
        }

        internal void FindAndCopyFileToWorkingDirectory(string path, string fileName, string assemblyName = "", bool overwrite = false)
        {
            if (!overwrite)
            {
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
                    return;
            }
            FindAndCopy(path, fileName, assemblyName);
        }

        private void FindAndCopy(string path, string fileName, string assemblyName)
        {
            if (path == Directory.GetCurrentDirectory()) //safetycheck
            {
                path = Directory.GetParent(path).FullName;
                FindAndCopy(path, fileName, assemblyName);
                return;
            }
            var file = Path.Combine(path, fileName);
            try
            {
                if (!File.Exists(file))
                {
                    if (!string.IsNullOrEmpty(assemblyName))
                    {
                        var cd = new DirectoryInfo(path).Name;
                        if (cd == assemblyName)
                        {
                            Console.WriteLine($"File: {fileName} not found. Search stopped in folder {cd}");
                            return;
                        }
                    }
                    path = Directory.GetParent(path).FullName;
                    FindAndCopy(path, fileName, assemblyName);
                }
                else
                {
                    var targetFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    if (File.Exists(targetFile))
                        File.Delete(targetFile);
                    File.Copy(file, Path.Combine(Directory.GetCurrentDirectory(), fileName));
                }
            }
            catch
            {
                Console.WriteLine($"File: {fileName} not found. Search stopped in {path}");
            };
        }
        #endregion

        public static string ParsePathToExcludeFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);

                if (Directory.Exists(path))
                    return path;

                throw new Exception("Invalid path");
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., security issues, invalid path formats)
                return "Error determining path '" + path + "' type: " + ex.Message;
            }
        }

        public static int GetTotalFilesInDirectory(string directoryPath, params string[] filesToExclude)
        {
            int CountFilesRecursive(string directory, int amount)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(directory);
                foreach (var pattern in filesToExclude)
                {
                    files = files.Where(file => !Path.GetFileName(file).Equals(pattern, StringComparison.OrdinalIgnoreCase)
                                                 && !Path.GetExtension(file).Equals(pattern, StringComparison.OrdinalIgnoreCase));
                }
                amount = files.Count();

                var directories = Directory.GetDirectories(directory);

                foreach (var d in directories)
                {
                    amount += CountFilesRecursive(d, amount);
                }
                return amount;
            }

            return CountFilesRecursive(directoryPath, 0);
        }

        public static List<string> DeleteAllContent(string folderPath, params string[] excluded)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The directory '" + folderPath + "' does not exist.");
                return default;
            }
            var log = new List<string>();
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                if (excluded.Any(x => x.Equals(Path.GetFileNameWithoutExtension(file), StringComparison.OrdinalIgnoreCase)))
                    continue;

                try
                {
                    File.Delete(file);
                    log.Add("Deleted file: " + file);
                }
                catch (Exception ex)
                {
                    log.Add("Failed to delete file: " + file + ". Error: " + ex.Message);
                }
            }

            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                if (excluded.Any(x => x.Equals(directory.Split('\\').Last(), StringComparison.OrdinalIgnoreCase)))
                    continue;

                try
                {
                    Directory.Delete(directory, true); // 'true' ensures recursive deletion
                    log.Add("Deleted directory: " + directory);
                }
                catch (Exception ex)
                {
                    log.Add("Failed to delete directory: " + directory + ". Error: " + ex.Message);
                }
            }
            return log;
        }
    }
}
