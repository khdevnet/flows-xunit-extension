public static class ScenarioAttributeClassProvider
{
    public const string ClassName = "ScenarioAttribute";
    public const string TextFieldName = "Text";

    public static string Get()
        => @$"
public class {ClassName} : System.Attribute
{{
    public string {TextFieldName} {{ get; set; }}
}}";
}
