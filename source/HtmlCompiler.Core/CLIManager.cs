using System.Diagnostics;
using System.Runtime.InteropServices;
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
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = ((RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ? "pwsh" : "bash"),
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