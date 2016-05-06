# TemplateParser
Template Parser is template engine written in C# using .NET Core </br>
The Template Parser Class can perform from simple text substitution or complex object substitution

### DOCS
---------------------------------------------------------------------------------------------------
#### Placeholder
The Template parser library allows for two types of placeholders either braces or brackets.

```csharp
public enum Placeholder
{
    Brace,
    Bracket
}
```

#### Template
The template can be any type of string
```csharp
string template = "Hello my name is [Name] [LastName]";
string template = "Hello my name is {Name} {LastName}";
```

#### Render
```csharp
/// <summary>Render the template using the supplied variable values</summary>
/// <param name="variables">Variables that can be substituted in the template string</param>
/// <param name="placeholder"></param>
/// <returns>The rendered template string</returns>
public string Render(IDictionary<string, PropertyMetaData> variables, Placeholder placeholder)
{
    return Render(TemplateString, variables, placeholder);
}
```
```csharp
/// <summary>Render the supplied template string using the supplied variable values</summary>
/// <param name="templateString">The template string to render</param>
/// <param name="variables">Variables that can be substituted in the template string</param>
/// <param name="placeholder"></param>
/// <returns>The rendered template string</returns>
public static string Render(string templateString, IDictionary<string, PropertyMetaData> variables, Placeholder placeholder)
{
    if (templateString == null)
        throw new ArgumentNullException(nameof(templateString));

    var pattern = GetSearchPattern(placeholder);

    return Regex.Replace(templateString, pattern, match =>
    {
        switch (match.Value[0])
        {
            case '\\':
                if (EscapeChars.ContainsKey(match.Value[1]))
                    return EscapeChars[match.Value[1]];
                break;
            case '{':
                if (variables.ContainsKey(match.Groups[1].Value))
                    return GetValue(variables, match.Groups[1].Value);
                break;
            case '[':
                if (variables.ContainsKey(match.Groups[1].Value))
                    return GetValue(variables, match.Groups[1].Value);
                break;
        }
        return "";
    }, RegexOptions.IgnoreCase);
}
```

#### PropertyMetaData
PropertyMetaData class will hold the value and the data type of the object, this is usefull if you want to sanitize the property
```csharp
public class PropertyMetaData
{
    public Type Type { get; set; }
    public object Value { get; set; }

    public static object SanitizeProperty(PropertyMetaData propertyMetaData)
    {
        ...
    }
}
```

#### Samples
Setting up the Dictionary
```csharp
Dictionary<string, PropertyMetaData> variableValues = new Dictionary<string, PropertyMetaData>
{
    {"Name", new PropertyMetaData() {Type = typeof (string), Value = "Jon"}},
    {"LastName", new PropertyMetaData() {Type = typeof (string), Value = "Doe"}},
};
```
Using the contructor to set the template
```csharp
var template = @"Hello my name is {Name} {LastName}";
var parser = new TemplateParser(template);

var result = parser.Render(variableValues, Placeholder.Brace);
```
or pass in the template as part of the overload
```csharp
var template = @"Hello my name is {Name} {LastName}";

var result = TemplateParser.Render(template, variableValues, Placeholder.Brace);
```

when using an complex object, there is an helper class called TypeExtensions that will allow you to get all the properties from the object and store it in an dictionary
```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public int PostalCode { get; set; }
    public Person Person { get; set; } // Circular property
}

var person = new Person
{
    FirstName = "Jon",
    LastName = "Doe",
    Address = new Address
    {
        Street = "Melkbos",
        PostalCode = 90210,
        Person = new Person
        {
            FirstName = "Jon",
            LastName = "Doe",
        }
    }
};

var variableValues = person.GetProperties();
var template = @"Hello my name is {Name} {LastName}";

var result = TemplateParser.Render(template, variableValues, Placeholder.Brace);

```