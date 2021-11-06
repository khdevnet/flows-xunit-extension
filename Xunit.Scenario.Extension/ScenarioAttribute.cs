using System;

namespace Xunit.Scenario.Extension
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
