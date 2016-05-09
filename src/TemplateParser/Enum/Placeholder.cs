using System.ComponentModel;

namespace TemplateParser
{
    public enum Placeholder
    {
        [Description(@"\{([a-z0-9_.\-]+)\}")]
        Brace,
        [Description(@"\[([a-z0-9_.\-]+)\]")]
        Bracket
    }
}