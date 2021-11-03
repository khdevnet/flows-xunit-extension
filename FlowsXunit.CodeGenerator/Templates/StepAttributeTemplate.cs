public static class StepAttributeTemplate
{
    public const string ClassName = "StepAttribute";

    public static string GetSource()
        => $@"
using Xunit;

public class {ClassName} : FactAttribute
{{
    public {ClassName}()
    {{
    }}

    public {ClassName}(string name)
    {{
        DisplayName = name;
    }}
}}";
}
