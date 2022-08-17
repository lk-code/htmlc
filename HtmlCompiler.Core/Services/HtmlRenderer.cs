using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core.Services
{
    public class HtmlRenderer : IHtmlRenderer
    {
        public async Task<string> RenderAsync(string sourceFile)
        {
            string content = await this.LoadFileContent(sourceFile);

            return content;
        }

        private async Task<string> LoadFileContent(string sourceFile)
        {
            string content = string.Empty;

            using (StreamReader streamReader = new StreamReader(sourceFile))
            {
                content = await streamReader.ReadToEndAsync();
            }

            return content;
        }
    }
}
