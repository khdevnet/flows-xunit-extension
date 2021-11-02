public static class StepAttributeClassProvider
{
    public static string Get()
        => @"
using Xunit;

public class StepAttribute : FactAttribute
{
    public StepAttribute()
    {
    }

    public StepAttribute(string name)
    {
        DisplayName = name;
    }
}";
}
