using System.Reflection;
using System.Text.Json;
using Cocona;
using Cocona.Builder;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using FluentDataBuilder.Microsoft.Extensions.Configuration;
using HtmlCompiler.Commands;
using HtmlCompiler.Config;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace HtmlCompiler;

class Program
{
    static void Main(string[] args)
    {
        string userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".htmlc");
        string userCacheDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".htmlc-cache");

        // user config file
        // if not exists => create
        EnsureUserConfigFile(userConfigPath);

        // ensure cache directory
        EnsureUserCacheDirectory(userCacheDirectoryPath);

        CoconaAppBuilder? builder = CoconaApp.CreateBuilder(args, options =>
        {
            
        });
        IDataBuilder configBuilder = new DataBuilder();

        // load appsettings
        using Stream appsettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HtmlCompiler.appsettings.json")!;
        using StreamReader appsettingsReader = new(appsettingsStream);
        string appsettingsJson = appsettingsReader.ReadToEnd();
        configBuilder = DataBuilder.Merge(configBuilder, new DataBuilder().LoadFrom(appsettingsJson));

        ILogger<Program> logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
        logger.LogTrace("##################################################");
        builder.Services.AddLogging(loggingBuilder =>
        {
            // configure Logging with NLog
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddNLog();
        });

        string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "htmlc.log");
        logger.LogInformation($"Log-File: {logFilePath}");

        // add user configuration
        logger.LogTrace($"add user configuration file: '{userConfigPath}'");
        configBuilder = DataBuilder.Merge(configBuilder, new DataBuilder().LoadFrom(new StreamReader(userConfigPath).ReadToEnd()));

        builder.Services.AddTransient<IConfigurationManager>(x => new Config.ConfigurationManager(userConfigPath, x.GetRequiredService<IFileSystemService>()));

        // add services
        builder.Services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
        builder.Services.AddTransient<IFileWatcher, FileWatcher>();
        builder.Services.AddTransient<IStyleManager, StyleManager>();
        builder.Services.AddTransient<IProjectManager, ProjectManager>();
        builder.Services.AddTransient<IFileSystemService, FileSystemService>();
        builder.Services.AddTransient<IResourceLoader, ResourceLoader>();
        builder.Services.AddTransient<ITemplateManager, TemplateManager>();
        builder.Services.AddTransient<IHttpClientService, HttpClientService>();
        builder.Services.AddTransient<ICLIManager, CLIManager>();
        builder.Services.AddTransient<IDependencyManager, DependencyManager>();
        builder.Services.AddTransient<ITemplatePackagingService, TemplatePackagingService>();
        builder.Services.AddTransient<IZipArchiveProvider, ZipArchiveProvider>();
        builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // add dependencies
        builder.Services.AddTransient<IDependencyObject, SassDependency>();
        builder.Services.AddTransient<IDependencyObject, LessDependency>();
        builder.Services.AddTransient<IDependencyObject, NodeDependency>();

        // add global core variables
        configBuilder.Add("Core", new DataBuilder()
            .Add("UserConfigPath", userConfigPath)
            .Add("UserCacheDirectoryPath", userCacheDirectoryPath));

        // load FluentDataBuilder with appsettings and user-configuration to IConfiguration
        builder.Configuration.AddConfiguration(configBuilder.ToConfiguration());

        CoconaApp? app = builder.Build();

        // store final configuration for debug
        string finalConfiguration = (app.Configuration as Microsoft.Extensions.Configuration.ConfigurationManager)!.GetDebugView();
        logger.LogTrace(finalConfiguration);

        app.AddCommands<HtmlcCommand>();
        app.AddCommands<EnvironmentRootCommand>();
        app.AddCommands<TemplateRootCommand>();

        app.Run();
    }

    private static void EnsureUserCacheDirectory(string userCacheDirectoryPath)
    {
        if (!Directory.Exists(userCacheDirectoryPath))
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(userCacheDirectoryPath);
            directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }
    }

    private static void EnsureUserConfigFile(string userConfigPath)
    {
        if (!File.Exists(userConfigPath))
        {
            using StreamWriter sw = File.CreateText(userConfigPath);
            ConfigModel basicConfiguration = ConfigModel.GetBasicConfig();
            string basicJsonConfiguration = JsonSerializer.Serialize(basicConfiguration);
            sw.WriteLine(basicJsonConfiguration);

            File.SetAttributes(userConfigPath, FileAttributes.Hidden);
        }
    }
}