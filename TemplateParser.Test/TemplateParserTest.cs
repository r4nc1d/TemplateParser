using System;
using System.Collections.Generic;
using Xunit;

namespace TemplateParser.Test
{
    public class TemplateParserTest
    {
        private readonly Dictionary<string, PropertyMetaData> _variableValues;
        public TemplateParserTest()
        {
            _variableValues = new Dictionary<string, PropertyMetaData>
            {
                {"Name", new PropertyMetaData {Type = typeof (string), Value = "Jon"}},
                {"LastName", new PropertyMetaData {Type = typeof (string), Value = "Doe"}}
            };
        }

        // Given i have an template


        // When i parse it
        // Then i should be able to match the string 
        [Fact]
        public void ParseTemplateConstructor_ShouldBeAbleToMatch_Test()
        {
            // Given
            var template = @"Hello my name is {Name} {LastName}";
            var parser = new TemplateParser(template);

            // When
            var result = parser.Render(_variableValues, Placeholder.Brace);

            // Then
            Assert.Equal("Hello my name is Jon Doe", result);
        }

        // Given i have an empty template
        // When i try to parse it 
        // Then should throw an exception
        [Theory]
        [InlineData(null)]
        public void Constructor_InvalidName_ExceptionThrown(string name)
        {
            Assert.Throws<ArgumentNullException>(() => new TemplateParser(name));
        }

        // Given i have an template with either an brace or bracket for an placeholder
        // When i try to parse it
        // Then i should be able to match the string
        [Theory]
        [InlineData(@"Hello my name is [Name] [LastName]", Placeholder.Bracket)]
        [InlineData(@"Hello my name is {Name} {LastName}", Placeholder.Brace)]
        public void ParseTemplate_ShouldBeAbleToMatch_Test(string template, Placeholder placeholder)
        {
            // When
            var result = TemplateParser.Render(template, _variableValues, placeholder);

            // Then
            Assert.Equal("Hello my name is Jon Doe", result);
        }

        // Given i have a search pattern
        // When the placeholder is either brace or bracket 
        // Then i should be able to match the pattern
        [Theory]
        [InlineData(Placeholder.Brace, @"\{([a-z0-9_.\-]+)\}")]
        [InlineData(Placeholder.Bracket, @"\[([a-z0-9_.\-]+)\]")]
        public void GetSearchPattern_ShouldBeAbleToMatch_Test(Placeholder placeholder, string pattern)
        {
            // When
            var result = placeholder.GetSearchPattern();

            // Then
            Assert.Equal(pattern, result);
        }

        // Given i have an empty template
        // When i try to parse it 
        // Then should throw an exception
        [Fact]
        public void ParseTemplate_EmptyTemplate_ShouldThrow_Test()
        {
            Assert.Throws<ArgumentNullException>(() => TemplateParser.Render(null, _variableValues, Placeholder.Brace));
        }
    }
}
