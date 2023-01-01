using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HtmlCompiler.Core.Extensions;
using HtmlCompiler.Core.Interfaces;

namespace HtmlCompiler.Core
{
    public class HtmlRenderer : IHtmlRenderer
    {
        public async Task<string> RenderHtmlAsync(string sourceFullFilePath)
        {
            string baseDirectory = this.GetBaseDirectory(sourceFullFilePath);
            string content = await this.LoadFileContent(sourceFullFilePath);

            // replace @Layout=...
            content = await this.ReplaceLayoutPlaceholderAsync(content, baseDirectory);

            // replace all @File=...
            content = await this.ReplaceFilePlaceholdersAsync(content, baseDirectory);

            return content;
        }

        private async Task<string> ReplaceFilePlaceholdersAsync(string content, string baseDirectory)
        {
            Regex fileTagRegex = new Regex(@"@File=([^\s]+)", RegexOptions.None, TimeSpan.FromMilliseconds(100));

            foreach (Match match in fileTagRegex.Matches(content))
            {
                string fileValue = match.Groups[1].Value;

                string fullPath = Path.Combine(baseDirectory, fileValue);

                // render the new file and return the rendered content
                string fileContent = await this.RenderHtmlAsync(fullPath);

                content = content.Replace(match.Value, fileContent);
            }

            return content;
        }

        private async Task<string> ReplaceLayoutPlaceholderAsync(string content, string baseDirectory)
        {
            int layoutIndex = content.IndexOf("@Layout");
            if (layoutIndex < 0)
            {
                return content;
            }

            int lineBreakIndex = content.IndexOf(Environment.NewLine, layoutIndex);
            if (lineBreakIndex < 0)
            {
                return content;
            }

            string layoutValue = content.Substring(layoutIndex + 8, lineBreakIndex - layoutIndex - 8).Trim();

            string fullPath = Path.Combine(baseDirectory, layoutValue);

            string layoutContent = await File.ReadAllTextAsync(fullPath);

            layoutContent = layoutContent.Replace("@Body", content);

            string output = string.Join(Environment.NewLine, layoutContent.Split(Environment.NewLine)
                .Where(x => x.Trim().ToLowerInvariant().StartsWith("@layout") != true)
                .ToArray());

            return output;
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

        private string GetBaseDirectory(string sourceFile)
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