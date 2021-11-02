public static class ScenarioAttributeClassProvider
{
    public static string Get()
        => @"
public class ScenarioAttribute : System.Attribute
{
    public string Text { get; set; }
}";
}
