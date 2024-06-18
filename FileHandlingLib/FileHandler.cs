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
        /// <param name="limitScopeToProject">Optional. Limits the search scope to the directory named after the current executing assembly's name. Default is true.</param>

        public static void FindAndCopyFileToWorkingDirectory(string path, string fileName, bool limitScopeToProject = true)
        {
            _fileHandling.FindAndCopyFileToWorkingDirectory(path, fileName, limitScopeToProject);
        }
        /// <summary>
        /// Retrieves the directory path of the currently executing assembly.
        /// </summary>
        /// <returns>
        /// A string containing the path of the directory where the executing assembly is located.
        /// This path includes the assembly name as the last folder in the path.
        public static string FindAssemblyDirectory()
        {
            return _fileHandling.GetAssemblyDirectory();
        }
    }
}
