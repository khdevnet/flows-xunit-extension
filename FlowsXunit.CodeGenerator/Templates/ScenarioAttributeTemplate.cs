public static class ScenarioAttributeTemplate
{
    public const string ClassName = "ScenarioAttribute";
    public const string TextFieldName = "Text";

    public static string GetSource()
        => @$"
public class {ClassName} : System.Attribute
{{
    public {ClassName}(string {TextFieldName.ToLower()}) 
    {{
       {TextFieldName} = {TextFieldName.ToLower()};
    }}
    public string {TextFieldName} {{ get; set; }}
}}";
}
