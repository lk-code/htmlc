using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace HtmlCompiler.Core
{
    public class HtmlRenderer : IHtmlRenderer
    {
        private string? _layoutFile = null;
        private string? _baseDirectory = null;
        private string _sourceFileContent = null!;

        public async Task<string> RenderAsync(string sourceFile, string outputFile)
        {
            this._baseDirectory = GetBaseDirectory(sourceFile);
            await LoadSourceContentAsync(sourceFile);

            string renderedContent = await this.RenderAsync();

            return renderedContent;
        }

        private async Task<string> RenderAsync()
        {
            string layoutContent = string.Empty;
            if (!string.IsNullOrEmpty(this._layoutFile))
            {
                // load layout
                layoutContent = await this.LoadFileContent(this._layoutFile);
            }

            if (string.IsNullOrEmpty(layoutContent))
            {
                return this._sourceFileContent;
            }

            string renderedContent = layoutContent.Replace("@Body", this._sourceFileContent);

            return renderedContent;
        }

        private async Task LoadSourceContentAsync(string sourceFile)
        {
            this._sourceFileContent = await this.LoadFileContent(sourceFile);

            string[] lines = this._sourceFileContent.ToLines();

            this._layoutFile = this.GetLayoutFileFromContent(lines);
            lines = lines.Where(l => !l.StartsWith("@Layout")).ToArray();

            this._sourceFileContent = lines.ToContent();
        }

        private string GetLayoutFileFromContent(string[] lines)
        {
            string? layoutLine = lines.SingleOrDefault(l => l.StartsWith("@Layout"));

            if (!string.IsNullOrEmpty(layoutLine))
            {
                return $"{this._baseDirectory}{Path.DirectorySeparatorChar}{layoutLine.Substring(layoutLine.IndexOf("=") + 1)}";
            }

            return string.Empty;
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

        private static string GetBaseDirectory(string sourceFile)
        {
            string? baseDirectory = Path.GetDirectoryName(sourceFile);

            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new FileNotFoundException($"\"{nameof(baseDirectory)}\" cannot be NULL or empty.", nameof(baseDirectory));
            }

            return baseDirectory;
        }
    }
}
