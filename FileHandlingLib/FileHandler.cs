using System.Reflection;

namespace Librac.FileHandlingLib
{
    /// <summary>
    /// LIbrary for dealing with files
    /// </summary>
    public static class FileHandler 
    {
        private static readonly FileHandlingMethods _fileHandling = new FileHandlingMethods();
        /// <summary>
        /// Sets the ReadOnly attribute to all files in the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        public static void ApplyReadOnlyToDirectory(string directoryPath)
        {
            _fileHandling.ApplyReadOnlyToDirectory(directoryPath);
        }
        /// <summary>
        /// Removes the ReadOnly attribute from all files in the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        public static void RemoveReadOnlyFromDirectory(string directoryPath)
        {
            _fileHandling.RemoveReadOnlyFromDirectory(directoryPath);
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
            _fileHandling.CopyFileTo(fileFullName, destinationFullName, overwrite); 
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
            _fileHandling.MoveFileTo(fileFullName, destinationFullName, overwrite);
        }

        /// <summary>
        /// Locates a specified file within a given path and copies it to the working directory. If the file is not found in the specified path, 
        /// the search will proceed up the directory tree recursively. The search is optionally  limited to a directory named after the executing assembly.
        /// </summary>
        /// <param name="path">The directory path where the file search begins.</param>
        /// <param name="fileName">The name of the file to search for and copy.</param>
        /// <param name="assemblyName">The current executing assembly for which the directory path is required. In general use: Assembly.GetExecutingAssembly().GetName().Name</param>

        public static void FindAndCopyFileToWorkingDirectory(string path, string fileName, string assemblyName)
        {
            _fileHandling.FindAndCopyFileToWorkingDirectory(path, fileName, assemblyName);
        }
        /// <summary>
        /// Retrieves the directory path of the specified executing assembly within the given working directory.
        /// </summary>
        /// <param name="currentWorkingDirectory">The working directory in which to search for the assembly name.</param>
        /// <param name="assemblyName">The current executing assembly for which the directory path is required. In general use: Assembly.GetExecutingAssembly().GetName().Name</param>
        /// <returns>
        /// A string containing the path of the directory where the specified executing assembly is located.
        /// This path includes the assembly name as the last folder in the path.
        public static string FindAssemblyDirectory(string currentWorkingDirectory, string assemblyName)
        {
            return _fileHandling.GetAssemblyDirectory(currentWorkingDirectory, assemblyName);
        }
    }
}
