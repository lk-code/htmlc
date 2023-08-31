using System.Text.Json;
using Cocona;
using Cocona.Builder;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler.Commands;
using HtmlCompiler.Config;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Dependencies;
using HtmlCompiler.Core.Interfaces;
using HtmlCompiler.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlCompiler;

static class Program
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

        CoconaAppBuilder? builder = CoconaApp.CreateBuilder(args);

        // add user configuration
        builder.Configuration.AddJsonStream(new StreamReader(userConfigPath).BaseStream);

        builder.Services.AddTransient<IConfigurationManager>(x =>
            new Config.ConfigurationManager(userConfigPath, x.GetRequiredService<IFileSystemService>()));

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

        builder.Services.AddTransient<IDependencyObject, SassDependency>();
        builder.Services.AddTransient<IDependencyObject, LessDependency>();
        builder.Services.AddTransient<IDependencyObject, NodeDependency>();

        IDataBuilder dataBuilder = new DataBuilder();
        dataBuilder.Add("Core", new DataBuilder()
            .Add("UserConfigPath", userConfigPath)
            .Add("UserCacheDirectoryPath", userCacheDirectoryPath));

        string jsonString = dataBuilder.Build().RootElement.GetRawText();
        using MemoryStream coreConfigJsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
        builder.Configuration.AddJsonStream(coreConfigJsonStream);

        CoconaApp? app = builder.Build();

        app.AddCommands<HtmlcCommand>();
        app.AddCommands<EnvironmentCommand>();

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