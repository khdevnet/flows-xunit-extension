using Xunit;

namespace Xunit.Scenario
{
    public class StepAttribute : FactAttribute
    {
        public StepAttribute()
        {
        }

        public StepAttribute(string name)
        {
            DisplayName = name;
        }
    }
}
