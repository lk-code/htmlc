using Cocona;
using Cocona.Builder;
using HtmlCompiler.Commands;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

CoconaAppBuilder? builder = CoconaApp.CreateBuilder();

// user config file
string userConfigFileFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".htmlc");

// if not exists => create
if (!File.Exists(userConfigFileFullPath))
{
    using (StreamWriter sw = File.CreateText(userConfigFileFullPath))
    {
        sw.WriteLine("{}");
    }
    File.SetAttributes(userConfigFileFullPath, FileAttributes.Hidden);
}

builder.Configuration
    .AddJsonFile("appsettings.json", false, false)
    .AddJsonStream(new StreamReader(userConfigFileFullPath).BaseStream);

builder.Services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
builder.Services.AddTransient<IHtmlWatcher, HtmlWatcher>();
builder.Services.AddTransient<IStyleCompiler, StyleCompiler>();

CoconaApp? app = builder.Build();

app.AddCommands<HtmlcCommand>();

app.Run();