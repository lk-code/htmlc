using System.Text.Json;
using Cocona;
using Cocona.Builder;
using HtmlCompiler;
using HtmlCompiler.Commands;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

CoconaAppBuilder? builder = CoconaApp.CreateBuilder();

// user config file
// if not exists => create
if (!File.Exists(Globals.USER_CONFIG))
{
    using (StreamWriter sw = File.CreateText(Globals.USER_CONFIG))
    {
        ConfigModel basicConfiguration = ConfigModel.GetBasicConfig();
        string basicJsonConfiguration = JsonSerializer.Serialize(basicConfiguration);
        sw.WriteLine(basicJsonConfiguration);
    }
    File.SetAttributes(Globals.USER_CONFIG, FileAttributes.Hidden);
}

builder.Configuration.AddJsonStream(new StreamReader(Globals.USER_CONFIG).BaseStream);

builder.Services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
builder.Services.AddTransient<IHtmlWatcher, HtmlWatcher>();
builder.Services.AddTransient<IStyleCompiler, StyleCompiler>();

CoconaApp? app = builder.Build();

app.AddCommands<HtmlcCommand>();
app.AddCommands<ConfigCommand>();

app.Run();
