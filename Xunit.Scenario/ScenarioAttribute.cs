using System;

namespace Xunit.Scenario
{
    public class ScenarioAttribute : Attribute
    {
        public ScenarioAttribute(string text)
        {
            Text = text;
        }
        public string Text { get; set; }
    }
}
