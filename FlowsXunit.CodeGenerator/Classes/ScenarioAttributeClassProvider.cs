public static class ScenarioAttributeClassProvider
{
    public const string ClassName = "ScenarioAttribute";
    public const string TextFieldName = "Text";

    public static string Get()
        => @$"
public class {ClassName} : System.Attribute
{{
    constructor(string {TextFieldName.ToLower()}) 
    {{
       {TextFieldName} = {TextFieldName.ToLower()};
    }}
    public string {TextFieldName} {{ get; set; }}
}}";
}
