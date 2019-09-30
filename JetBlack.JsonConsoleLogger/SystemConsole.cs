#nullable enable

using System.IO;

namespace JetBlack.JsonConsoleLogger
{
    internal class SystemConsole : ISystemConsole
    {
        private readonly TextWriter _textWriter;

        public SystemConsole(bool stdErr = false)
        {
            _textWriter = stdErr ? System.Console.Error : System.Console.Out;
        }

        public void Write(string message)
        {
            _textWriter.Write(message);
        }
    }
}