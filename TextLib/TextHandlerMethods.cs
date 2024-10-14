using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librac.TextLib
{
    internal class TextHandlerMethods
    {
        #region BREAK TEXT INTO LINES
        internal List<string> BreakRawTextIntoLines(string text, int lineLength)
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

        private string CreateLine(ref int index, int lineLength, string[] words, StringBuilder sb)
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
            return sb.ToString().Trim() + Environment.NewLine;
        }
        #endregion

        #region LINE DIFERENCES
        internal string[] GetLineDifferences(string oldText, string newText, int treshold = 1, bool allowEmptyLines = false)
        {
            if (allowEmptyLines)
                return GetLineDifferences(SplitTextIntoLines(oldText), SplitTextIntoLines(newText), treshold, allowEmptyLines);
            else
                return GetLineDifferences(SplitTextIntoNotEmptyLines(oldText), SplitTextIntoNotEmptyLines(newText), treshold, allowEmptyLines);
        }

        internal string[] SplitTextIntoNotEmptyLines(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (lines.Length > 0 && lines[lines.Length - 1] == "")
            {
                return lines.Take(lines.Length - 1).ToArray();
            }
            return lines;
        }

        internal string[] SplitTextIntoLines(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToArray();
            if (lines.Length > 0 && lines[lines.Length - 1] == "")
            {
                return lines.Take(lines.Length - 1).ToArray();
            }
            return lines;
        }

        internal string[] GetLineDifferences(IEnumerable<string> oldText, IEnumerable<string> newText, int threshold = 1, bool allowEmptyLines = false)
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
        #endregion
    }
}
