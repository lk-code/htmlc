using FluentAssertions;
using HtmlCompiler.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace HtmlCompiler.Tests.Core.Extensions;

[TestClass]
public class JsonExtensionsTests
{
    [TestMethod]
    public void UpdateJsonProperty_WithEmptyJson_ReturnsCorrect()
    {
        string json = "{}";
        string key = "Application:Host";
        string value = "https://www.test.de";

        string result = json.UpdateJsonProperty(key, value);

        result.Should().NotBeNullOrEmpty();

        JObject.Parse(result)
            .SelectToken("Application")!
            .SelectToken("Host")!
            .ToString()
            .Should()
            .Be("https://www.test.de");
    }

    [TestMethod]
    public void UpdateJsonProperty_WithExistingJson_ReturnsCorrect()
    {
        string json = "{\"Application\":{\"Host\":\"http://www.test.de\"}}";
        string key = "Application:Host";
        string value = "https://www.test.link";

        string result = json.UpdateJsonProperty(key, value);

        result.Should().NotBeNullOrEmpty();
        
        JObject.Parse(result)
            .SelectToken("Application")!
            .SelectToken("Host")!
            .ToString()
            .Should()
            .Be("https://www.test.link");
    }

    [TestMethod]
    public void UpdateJsonProperty_WithPartialExistingJson_ReturnsCorrect()
    {
        string json = "{\"Application\":{\"Port\":8080}}";
        string key = "Application:Host";
        string value = "https://www.test.com";

        string result = json.UpdateJsonProperty(key, value);

        result.Should().NotBeNullOrEmpty();

        JObject.Parse(result)
            .SelectToken("Application")!
            .SelectToken("Port")!
            .ToString()
            .Should()
            .Be("8080");

        JObject.Parse(result)
            .SelectToken("Application")!
            .SelectToken("Host")!
            .ToString()
            .Should()
            .Be("https://www.test.com");
    }
}