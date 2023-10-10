namespace HtmlCompiler.Core.Interfaces;

public interface ICLIManager
{
    /// <summary>
    /// executes the given command directly on the console
    /// </summary>
    /// <param name="command">the command to execute</param>
    /// <returns></returns>
    string ExecuteCommand(string command);
}