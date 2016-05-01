using System;
using System.Collections.Generic;
using Xunit;

namespace TemplateParser.Test
{
    public class TemplateParserTest
    {
        private readonly Dictionary<string, object> _variableValues;
        public TemplateParserTest()
        {
            _variableValues = new Dictionary<string, object>
            {
                {"Name", "Jon"}, {"LastName", "Doe"},
            };
        }

        [Fact]
        public void ParseTemplateConstructor_ShouldBeAbleToMatch_Test()
        {
            // Given
            var template = @"Hello my name is {Name} {LastName}";
            var parser = new TemplateParser(template);

            // When
            var result = parser.Render(_variableValues, Placeholder.Bracket);

            // Then
            Assert.Equal("Hello my name is Jon Doe", result);
        }

        [Theory()]
        [InlineData(null)]
        public void Constructor_InvalidName_ExceptionThrown(string name)
        {
            Assert.Throws<ArgumentNullException>(() => new TemplateParser(name));
        }

        [Theory()]
        [InlineData(@"Hello my name is [Name] [LastName]", Placeholder.Brace)]
        [InlineData(@"Hello my name is {Name} {LastName}", Placeholder.Bracket)]
        public void ParseTemplate_ShouldBeAbleToMatch_Test(string template, Placeholder placeholder)
        {
            // When
            var result = TemplateParser.Render(template, _variableValues, placeholder);

            // Then
            Assert.Equal("Hello my name is Jon Doe", result);
        }

        [Theory()]
        [InlineData(Placeholder.Bracket, @"\{([a-z0-9_.\-]+)\}")]
        [InlineData(Placeholder.Brace, @"\[([a-z0-9_.\-]+)\]")]
        public void GetSearchPattern_ShouldBeAbleToMatch_Test(Placeholder placeholder, string pattern)
        {
            var result = TemplateParser.GetSearchPattern(placeholder);
            Assert.Equal(pattern, result);
        }

        [Fact]
        public void ParseTemplate_EmptyTemplate_ShouldThrow_Test()
        {
            Assert.Throws<ArgumentNullException>(() => TemplateParser.Render(null, _variableValues));
        }
    }
}
