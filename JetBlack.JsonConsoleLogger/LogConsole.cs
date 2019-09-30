#nullable enable

using System.Text;

namespace JetBlack.JsonConsoleLogger
{
    internal class LogConsole : IConsole
    {
        private readonly StringBuilder _outputBuilder;
        private readonly ISystemConsole _systemConsole;

        public LogConsole(ISystemConsole systemConsole)
        {
            _outputBuilder = new StringBuilder();
            _systemConsole = systemConsole;
        }

        public void Write(string message)
        {
            _outputBuilder.Append(message);
        }

        public void WriteLine(string message)
        {
            Write(message);
            _outputBuilder.AppendLine();
        }

        public void Flush()
        {
            _systemConsole.Write(_outputBuilder.ToString());
            _outputBuilder.Clear();
        }
    }
}
