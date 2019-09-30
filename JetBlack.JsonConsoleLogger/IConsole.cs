#nullable enable

namespace JetBlack.JsonConsoleLogger
{
    internal interface IConsole
    {
        void Write(string message);
        void WriteLine(string message);
        void Flush();
    }
}
