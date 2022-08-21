using Cocona;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Commands
{
    public class HtmlcCommand
    {
        private readonly IHtmlRenderer _htmlRenderer;

        public HtmlcCommand(IHtmlRenderer htmlRenderer)
        {
            this._htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        }

        [Command("compile")]
        public async Task Compile([Argument(Description = "path to the source file")] string sourceFile,
            [Argument(Description = "path to the output file")] string outputFile)
        {
            string fullOutputFilePath = outputFile.FromSourceFilePath(sourceFile);

            Console.WriteLine($"compile file {sourceFile} to {fullOutputFilePath}");

            try
            {
                await this._htmlRenderer.RenderToFileAsync(sourceFile, fullOutputFilePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"file {sourceFile} not found");
            }
        }
    }
}
