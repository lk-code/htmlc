using System.Diagnostics;
using System.Text;
using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core;

public class CLIManager : ICLIManager
{
    public CLIManager()
    {
    }

    public string ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "bash", // Verwende eine Shell (z.B. bash) zum Ausführen des Befehls
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        StringBuilder outputBuilder = new StringBuilder();
        StringBuilder errorBuilder = new StringBuilder();

        using Process process = new Process();
        process.StartInfo = processInfo;

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        string output = outputBuilder.ToString();
        string error = errorBuilder.ToString();

        if (!string.IsNullOrEmpty(error))
        {
            throw new ConsoleExecutionException(error);
        }

        return output;
    }
}