using Cocona;
using Microsoft.Extensions.Configuration;

namespace HtmlCompiler.Commands;

public class EnvironmentCommand
{
    private readonly IConfiguration _configuration;

    public EnvironmentCommand(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [Command("check")]
    public async Task Check()
    {
        
    }

    [Command("setup")]
    public async Task Setup()
    {
        
    }
}