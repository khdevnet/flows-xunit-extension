using System;
using System.IO;
using Xunit;

namespace FlowsXunit.CodeGenerator.Test
{
    public class ConstantClasses
    {
        [Fact]
        public void ReadFromProjectDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
        }
    }
}
