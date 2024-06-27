using System.Collections.Generic;

namespace Librac.TextLib
{
    /// <summary>
    /// Text manipulation 
    /// </summary>
    public static class TextHandler
    {
        private static readonly TextHandlerMethods _handlerMethods = new TextHandlerMethods();

        /// <summary>
        /// Breaks a given text into a list of strings, where each string represents a line of text 
        /// up to a specified maximum length. This method handles word wrapping without breaking words.
        /// </summary>
        /// <param name="text">The input text to be broken into lines.</param>
        /// <param name="lineLength">The maximum length of each line.</param>
        /// <returns>A list of strings, each representing a line of the specified maximum length.</returns>
        public static List<string> BreakRawTextIntoLines(string text, int lineLength)
        {
            return _handlerMethods.BreakRawTextIntoLines(text, lineLength);
        }

        /// <summary>
        /// Splits the given text into lines, excluding any empty lines. This method is useful for preprocessing text to remove irrelevant whitespace lines. Will not work correctly if text doesn't have line breaks
        /// </summary>
        /// <param name="text">The text to split into non-empty lines.</param>
        /// <returns>An array of strings, each representing a non-empty line from the original text.</returns>
        public static string[] SplitTextIntoNotEmptyLines(string text)
        {
            return _handlerMethods.SplitTextIntoNotEmptyLines(text);
        }

        /// <summary>
        /// Splits the given text into lines. Will not work correctly if text doesn't have line breaks
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <returns>An array of strings, each representing a line from the original text.</returns>
        public static string[] SplitTextIntoLines(string text)
        {
            return _handlerMethods.SplitTextIntoLines(text);
        }

        /// <summary>
        /// Calculates the differences between two texts by comparing their lines. This method converts the texts into non-empty lines and then determines the differences.
        /// </summary>
        /// <param name="oldText">The original text to compare.</param>
        /// <param name="newText">The new text to compare.</param>
        /// <param name="threshold">The number of consecutive matching lines that need to be found before stopping the comparison. Default is 1</param>
        /// <param name="allowEmptyLines">Specifies whether empty lines should be included in the comparison. When set to true, all lines are considered; when false, empty lines are ignored. Default is false</param>
        /// <returns>An array of strings containing the lines that differ between the two texts, up to the specified threshold.</returns>
        public static string[] GetLineDifferences(string oldText, string newText, int threshold = 1, bool allowEmptyLines = false)
        {
            return _handlerMethods.GetLineDifferences(oldText, newText, threshold, allowEmptyLines);
        }

        /// <summary>
        /// Calculates the line differences between two sets of text lines. This method is used internally to compare arrays of lines.
        /// </summary>
        /// <param name="oldText">An enumerable of strings representing the lines of the original text.</param>
        /// <param name="newText">An enumerable of strings representing the lines of the new text.</param>
        /// <param name="threshold">The number of consecutive matching lines that need to be found before stopping the comparison. Default is 1</param>
        /// <param name="allowEmptyLines">Specifies whether empty lines should be included in the comparison. When set to true, all lines are considered; when false, empty lines are ignored. Default is false</param>
        /// <returns>An array of strings containing the lines that differ, considering the specified threshold.</returns>
        public static string[] GetLineDifferences(IEnumerable<string> oldText, IEnumerable<string> newText, int threshold = 1, bool allowEmptyLines = false)
        {
            return _handlerMethods.GetLineDifferences(oldText, newText, threshold, allowEmptyLines);
        }
    }
}
