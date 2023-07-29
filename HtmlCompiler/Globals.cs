using System;
namespace HtmlCompiler;

public static class Globals
{
    public static string USER_CONFIG = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".htmlc");
}