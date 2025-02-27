using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibRac.Text
{
    /// <summary>
    /// Text manipulation 
    /// </summary>
    public static class TextHandler
    {
        /// <summary>
        /// Breaks a given text into a list of strings, where each string represents a line of text up to a specified maximum length. 
        /// This method handles word wrapping without breaking words and will work even if no line brakes are in text.
        /// </summary>
        /// <param name="text">The input text to be broken into lines.</param>
        /// <param name="lineLength">The maximum length of each line.</param>
        /// <returns>A list of strings, each representing a line of the specified maximum length.</returns>
        public static List<string> BreakRawTextIntoLines(string text, int lineLength)
        {
            int index = 0;
            List<string> lines = new List<string>();
            var sb = new StringBuilder();

            string[] words = text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Split(' ');

            while (index < words.Length)
            {
                lines.Add(CreateLine(ref index, lineLength, words, sb));
            }
            return lines;
        }

        private static string CreateLine(ref int index, int lineLength, string[] words, StringBuilder sb)
        {
            sb.Clear();
            while (index < words.Length)
            {
                if (sb.Length + words[index].Length <= lineLength)
                {
                    sb.Append(words[index] + " ");
                    index++;
                }
                else
                    break;

            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Splits the given text into lines. Will not work correctly if text doesn't have line breaks
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <returns>An array of strings, each representing a line from the original text.</returns>
        public static string[] SplitTextIntoLines(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToArray();
            if (lines.Length > 0 && lines[lines.Length - 1] == "")
            {
                return lines.Take(lines.Length - 1).ToArray();
            }
            return lines;
        }

        /// <summary>
        /// Splits the given text into lines, excluding any empty lines. This method is useful for preprocessing text to remove irrelevant whitespace lines. 
        /// Will not work correctly if text doesn't have line breaks
        /// </summary>
        /// <param name="text">The text to split into non-empty lines.</param>
        /// <returns>An array of strings, each representing a non-empty line from the original text.</returns>
        public static string[] SplitTextIntoNotEmptyLines(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (lines.Length > 0 && lines[lines.Length - 1] == "")
            {
                return lines.Take(lines.Length - 1).ToArray();
            }
            return lines;
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
            if (allowEmptyLines)
                return GetLineDifferences(SplitTextIntoLines(oldText), SplitTextIntoLines(newText), threshold, allowEmptyLines);
            else
                return GetLineDifferences(SplitTextIntoNotEmptyLines(oldText), SplitTextIntoNotEmptyLines(newText), threshold, allowEmptyLines);
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
            int index_old = 0;
            int index_new = 0;
            string[] oldt;
            string[] newt;
            int localThreshold = threshold;

            if (allowEmptyLines)
            {
                oldt = oldText.Reverse().ToArray();
                newt = newText.Reverse().ToArray();
            }
            else
            {
                oldt = oldText.Where(x => !string.IsNullOrWhiteSpace(x)).Reverse().ToArray();
                newt = newText.Where(x => !string.IsNullOrWhiteSpace(x)).Reverse().ToArray();
            }

            if (oldt.Length == 0)
                return newt.Reverse().ToArray();
            if (newt.Length == 0)
                return new string[0];

            while (localThreshold > 0)
            {
                if (oldt[index_old] == newt[index_new])
                {
                    index_old++;
                    index_new++;
                    localThreshold--;
                }
                else
                {
                    index_new++;
                    localThreshold = threshold;
                }
                if (index_new == newt.Length || index_old == oldt.Length)
                    break;
            }
            var elements = index_new - index_old;
            return newt.Take(elements).Reverse().ToArray();
        }
    }
}
