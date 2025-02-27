using System.Text;

namespace LibRac.Shell.Logger
{
    internal class StringBuilderLogger : IShellLogger
    {
        private readonly StringBuilder _stringBuilder;

        public StringBuilderLogger(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public void Log(string message)
        {
            _stringBuilder.AppendLine(message);
        }
    }
}
