using Cocona;
using Cocona.Builder;
using HtmlCompiler.Commands;
using HtmlCompiler.Core;
using HtmlCompiler.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

CoconaAppBuilder? builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
builder.Services.AddTransient<IHtmlWatcher, HtmlWatcher>();

CoconaApp? app = builder.Build();

app.AddCommands<HtmlcCommand>();

app.Run();