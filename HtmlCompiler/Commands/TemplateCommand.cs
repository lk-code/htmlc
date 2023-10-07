using Cocona;

namespace HtmlCompiler.Commands;

[HasSubCommands(typeof(TemplateCommands), "template", Description = "template sub-commands")]
public class TemplateRootCommand
{
}

public class TemplateCommands
{
    [Command("create")]
    public async Task Create()
    {
        
    }
    
    // public async Task Upload()
    // {
    //     
    // }
}