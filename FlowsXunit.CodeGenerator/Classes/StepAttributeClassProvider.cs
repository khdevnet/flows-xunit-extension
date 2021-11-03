public static class StepAttributeClassProvider
{
    public const string ClassName = "StepAttribute";

    public static string Get()
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
