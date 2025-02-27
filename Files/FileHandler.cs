using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace LibRac.Files
{
    /// <summary>
    /// Library for dealing with files
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Sets the ReadOnly attribute to all files in the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        public static void ApplyReadOnlyToDirectory(string directoryPath)
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

        /// <summary>
        /// Removes the ReadOnly attribute from all files in the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        public static void RemoveReadOnlyFromDirectory(string directoryPath)
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

        /// <summary>
        /// Copies a file to a specified destination, with an option to overwrite the existing destination file.
        /// </summary>
        /// <param name="fileFullName">The full path of the file to copy.</param>
        /// <param name="destinationFullName">The full path of the destination where the file will be copied.</param>
        /// <param name="overwrite">A boolean indicating whether to overwrite the destination file if it already exists.</param>
        /// <remarks>
        /// If the source file does not exist, the method returns without performing any action.
        /// The method attempts to copy the file and will overwrite the destination if the 'overwrite' parameter is true.
        /// This method prints to the console whether the file was copied successfully or if an error occurred.
        /// </remarks>
        public static void CopyFileTo(string fileFullName, string destinationFullName, bool overwrite)
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

        /// <summary>
        /// Moves a file to a specified destination, optionally overwriting the destination file.
        /// </summary>
        /// <param name="fileFullName">The full path of the file to move.</param>
        /// <param name="destinationFullName">The full path of the destination where the file will be moved.</param>
        /// <param name="overwrite">A boolean indicating whether to overwrite the destination file if it already exists.</param>
        /// <remarks>
        /// If the source file does not exist, the method returns without performing any action. 
        /// If the 'overwrite' parameter is true and the destination file exists, the destination file will be deleted prior to moving.
        /// This method prints to the console whether the file was moved successfully or if an error occurred.
        /// </remarks>
        public static void MoveFileTo(string fileFullName, string destinationFullName, bool overwrite)
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

        /// <summary>
        /// Locates a specified file within a given path and copies it to the working directory. If the file is not found in the specified path, 
        /// the search will proceed up the directory tree recursively. The search is optionally  limited to a directory named after the executing assembly.
        /// </summary>
        /// <param name="startPath">The directory path where the file search begins.</param>
        /// <param name="fileName">The name of the file to search for and copy.</param>
        /// <param name="assembly">Optional: The current executing assembly. This will limit search at the root folder of the assembly. Default is null</param>
        /// <param name="overwrite">Optional: If set to false and file exist in working directory it will not overwrite stated file. Default is false</param>
        public static void FindAndCopyFileToWorkingDirectory(string startPath, string fileName, Assembly? assembly = null, bool overwrite = false)
        {
            string assemblyName = "";
            if (assembly != null)
                assemblyName = assembly.GetName().Name;

            if (!overwrite)
            {
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
                    return;
            }
            FindAndCopy(startPath, fileName, assemblyName);
        }

        private static void FindAndCopy(string path, string fileName, string assemblyName)
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


        /// <summary>
        /// Retrieves the directory path of the specified executing assembly within the given working directory.
        /// </summary>
        /// <param name="workingDirectory">The working directory in which to search for the assembly name.</param>
        /// <param name="assembly">The current executing assembly for which the directory path is required. In general use: Assembly.GetExecutingAssembly().GetName().Name</param>
        /// <returns>
        /// A string containing the path of the directory where the specified executing assembly is located.
        /// This path includes the assembly name as the last folder in the path.
        public static string FindAssemblyDirectory(string workingDirectory, Assembly assembly)
        { 
            var assemblyName = assembly.GetName().Name;
            var index = workingDirectory.IndexOf(assemblyName) + assemblyName.Length;
            return workingDirectory.Substring(0, index);
        }

        /// <summary>
        /// Parses a given file or directory path and returns the directory path excluding the file name. If the input is already a directory path, it returns the same.
        /// </summary>
        /// <param name="path">The file or directory path to parse.</param>
        /// <returns>The directory part of the path if the path is a file, the original path if it is a directory, or an error message if neither.</returns>
        /// <exception cref="Exception">Thrown when the provided path is neither a valid file nor a directory.</exception>
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

        /// <summary>
        /// Calculates the total number of files within a specified directory and all its subdirectories, excluding files that match any of the provided patterns.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to search within.</param>
        /// <param name="filesToExclude">An array of filenames or extensions to exclude from the count.</param>
        /// <returns>The total number of files in the directory after excluding specified files.</returns>
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

        /// <summary>
        /// Deletes all files and directories within the specified folder path, except for those specified in the excluded list. 
        /// </summary>
        /// <param name="folderPath">The path of the folder from which to delete content.</param>
        /// <param name="excluded">Files and directories to exclude from deletion.</param>
        /// <returns>A list of log entries indicating the deletion status of each file and directory; or null if the directory does not exist.</returns>
        /// <remarks>
        /// This method logs every delete operation, whether successful or failed, and returns these logs. 
        /// It continues deletion even if some deletions fail, logging each failure.
        /// </remarks>
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
