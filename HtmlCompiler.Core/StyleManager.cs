using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Core;

public class StyleManager : IStyleManager
{
    private readonly IConfiguration _configuration;
    private readonly IFileSystemService _fileSystemService;
    private readonly ICLIManager _cliManager;

    private string _sourceDirectoryPath = null!;
    private string _outputDirectoryPath = null!;

    public StyleManager(IConfiguration configuration,
        IFileSystemService fileSystemService,
        ICLIManager cliManager)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        _cliManager = cliManager ?? throw new ArgumentNullException(nameof(cliManager));
    }

    public async Task<string?> CompileStyleAsync(string sourceDirectoryPath, string outputDirectoryPath, string? styleSourceFilePath)
    {
        await Task.CompletedTask;
        
        this._sourceDirectoryPath = sourceDirectoryPath;
        this._outputDirectoryPath = outputDirectoryPath;
        
        if (string.IsNullOrEmpty(styleSourceFilePath))
        {
            return null;
        }

        if(!this._fileSystemService.FileExists(styleSourceFilePath))
        {
            throw new StyleNotFoundException($"style file not found at {styleSourceFilePath}");
        }

        string fileExtension = Path.GetExtension(styleSourceFilePath)
            .ToLowerInvariant();
        fileExtension.IsSupportedStyleFileOrThrow();
        fileExtension = fileExtension.TrimStart('.');
        
        string sourceFilePath = styleSourceFilePath;
        string sourceFileName = styleSourceFilePath.Replace(sourceDirectoryPath, "");
        string outputFileName = Path.ChangeExtension(sourceFileName, "css");
        string outputFilePath = $"{this._outputDirectoryPath}{outputFileName}";

        string? styleCompileCommandTemplate = this._configuration[$"style-commands:{fileExtension}"];
        if (styleCompileCommandTemplate is null)
        {
            throw new StyleCommandNotFoundException($"style compile command for '{fileExtension}' not found");
        }
        
        string styleCompileCommand = string.Format(styleCompileCommandTemplate, $"\"{sourceFilePath}\"", $"\"{outputFilePath}\"");
        
        this._cliManager.ExecuteCommand(styleCompileCommand);

        return outputFilePath;
    }
}