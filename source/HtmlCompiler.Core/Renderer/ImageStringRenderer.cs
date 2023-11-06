using System.Text.RegularExpressions;
using HtmlCompiler.Core.Interfaces;
using SkiaSharp;

namespace HtmlCompiler.Core.Renderer;

public class ImageStringRenderer : RenderingBase
{
    public const string IMAGESTRING_TAG = "@ImageString";
    private const float DEFAULT_FONTSIZE = 20.0f;
    private const string DEFAULT_BACKGROUND = "#ffffffff";
    private const string DEFAULT_FOREGROUND = "#ff000000";

    public ImageStringRenderer(RenderingConfiguration configuration,
        IHtmlRenderer htmlRenderer)
        : base(configuration,
            htmlRenderer)
    {
    }

    public override async Task<string> RenderAsync(string content)
    {
        await Task.CompletedTask;

        Regex regex = new(IMAGESTRING_TAG + @"\((.*?)\)");
            
        MatchCollection matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            string originalMatch = match.Value;
            string parametersContent = match.Groups[1].Value;
            
            string[] parameters = parametersContent.Split(",")
                .Select(x => x.Replace('"', ' ').Trim())
                .ToArray();

            string text = GetValueFromArray(parameters, 0, string.Empty);
            string backgroundColor = GetValueFromArray(parameters, 1, DEFAULT_BACKGROUND);
            string foregroundColor = GetValueFromArray(parameters, 2, DEFAULT_FOREGROUND);
            float fontSize = GetValueFromArray<float>(parameters, 3, DEFAULT_FONTSIZE);

            string base64Value = GetStringAsImageBase(text,
                fontSize,
                backgroundColor,
                foregroundColor);
            string base64 = $"data:image/png;base64, {base64Value}";

            content = content.Replace(originalMatch, base64);
        }

        return content;
    }
    
    private T GetValueFromArray<T>(string[] arr, int index, T defaultValue)
    {
        if (arr.Length > index)
        {
            string value = arr[index];
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        return defaultValue;
    }

    private string GetStringAsImageBase(string text,
        float textSize,
        string backgroundColor,
        string foregroundColor)
    {
        SKColor skBackgroundColor = SKColor.Parse(backgroundColor);
        SKColor skForegroundColor = SKColor.Parse(foregroundColor);

        using SKPaint paint = new();
        paint.TextSize = textSize;

        SKRect textBounds = new();
        paint.MeasureText(text, ref textBounds);
        int width = (int)Math.Ceiling(textBounds.Right) + 20;
        int height = (int)Math.Ceiling(textBounds.Height) + 20;

        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);

        canvas.Clear(skBackgroundColor);
        paint.Color = skForegroundColor;
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