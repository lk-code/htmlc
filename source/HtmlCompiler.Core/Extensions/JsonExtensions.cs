using FluentDataBuilder;
using FluentDataBuilder.Json;

namespace HtmlCompiler.Core.Extensions;

public static class JsonExtensions
{
    public static string UpdateJsonProperty(this string json, string key, object value)
    {
        IDataBuilder builder = new DataBuilder()
            .LoadFrom(json);

        builder[key] = value;

        string jsonOutput = builder.Build().RootElement.GetRawText();

        return jsonOutput;
    }
}