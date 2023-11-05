using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;
using SkiaSharp;

namespace HtmlCompiler.Core.Renderer;

public class ImageStringRenderer : RenderingBase
{
    public const string IMAGESTRING_TAG = "@ImageString";

    public ImageStringRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;

        Regex regex = new(IMAGESTRING_TAG + @"\(""(.*?)""\)");

        MatchCollection matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            string originalMatch = match.Value;
            string parameterValue = match.Groups[1].Value;

            string base64Value = GetStringAsImageBase(parameterValue, 20.0f);
            string base64 = $"data:image/png;base64, {base64Value}";

            content = content.Replace(originalMatch, base64);
        }

        return content;
    }

    private string GetStringAsImageBase(string text, float textSize)
    {
        SKColor backgroundColor = SKColors.White;
        SKColor foregroundColor = SKColors.Black;
        
        using SKPaint paint = new();
        paint.TextSize = textSize;

        SKRect textBounds = new();
        paint.MeasureText(text, ref textBounds);
        int width = (int)Math.Ceiling(textBounds.Right) + 20;
        int height = (int)Math.Ceiling(textBounds.Height) + 20;

        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);

        canvas.Clear(backgroundColor);
        paint.Color = foregroundColor;
        canvas.DrawText(text, 10, 30, paint);

        using SKImage img = SKImage.FromBitmap(bitmap);
        using SKImage encodedImg = SKImage.FromEncodedData(img.Encode());
        using SKData data = encodedImg.Encode();
        using MemoryStream stream = new(data.ToArray());

        byte[] imageBytes = stream.ToArray();
        string base64String = Convert.ToBase64String(imageBytes);
        
        return base64String;
    }
}