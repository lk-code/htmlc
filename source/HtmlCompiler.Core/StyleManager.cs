using HtmlCompiler.Core.Exceptions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HtmlCompiler.Core;

public class StyleManager : IStyleManager
{
    private readonly ILogger<StyleManager> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFileSystemService _fileSystemService;
    private readonly ICLIManager _cliManager;

    private string _sourceDirectoryPath = null!;
    private string _outputDirectoryPath = null!;

    public StyleManager(ILogger<StyleManager> logger,
        IConfiguration configuration,
        IFileSystemService fileSystemService,
        ICLIManager cliManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        try
        {
            this._cliManager.ExecuteCommand(styleCompileCommand);
        } catch (Exception e)
        {
            this._logger.LogError(e, $"error while executing style compile command '{styleCompileCommand}'");
        }

        return outputFilePath;
    }
}