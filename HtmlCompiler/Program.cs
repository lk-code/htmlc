using System.Text;
using System.Text.Json;
using Cocona;
using Cocona.Builder;
using FluentDataBuilder;
using FluentDataBuilder.Json;
using HtmlCompiler;
using HtmlCompiler.Commands;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlCompiler;

static class Program
{
    static void Main(string[] args)
    {
        string userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".htmlc");

        // user config file
        // if not exists => create
        if (!File.Exists(userConfigPath))
        {
            using (StreamWriter sw = File.CreateText(userConfigPath))
            {
                ConfigModel basicConfiguration = ConfigModel.GetBasicConfig();
                string basicJsonConfiguration = JsonSerializer.Serialize(basicConfiguration);
                sw.WriteLine(basicJsonConfiguration);
            }

            File.SetAttributes(userConfigPath, FileAttributes.Hidden);
        }
        
        CoconaAppBuilder? builder = CoconaApp.CreateBuilder(args);

        // add user configuration
        builder.Configuration.AddJsonStream(new StreamReader(userConfigPath).BaseStream);

        builder.Services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
        builder.Services.AddTransient<IFileWatcher, FileWatcher>();
        builder.Services.AddTransient<IStyleCompiler, StyleCompiler>();
        builder.Services.AddTransient<IProjectManager, ProjectManager>();
        builder.Services.AddTransient<IFileSystemService, FileSystemService>();
        builder.Services.AddTransient<IResourceLoader, ResourceLoader>();

        IDataBuilder dataBuilder = new DataBuilder();
        dataBuilder.Add("Core", new DataBuilder()
            .Add("UserConfigPath", userConfigPath));

        string jsonString = dataBuilder.Build().RootElement.GetRawText();
        using MemoryStream coreConfigJsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
        builder.Configuration.AddJsonStream(coreConfigJsonStream);
        
        CoconaApp? app = builder.Build();

        app.AddCommands<HtmlcCommand>();
        app.AddCommands<ConfigCommand>();

        app.Run();
    }
}