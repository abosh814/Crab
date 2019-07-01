using System;

namespace Crab
{
    public class RegexAlias : Attribute //regex alias
    {
        public string pattern;
        public RegexAlias(string regex)
        {
            pattern = regex;
        }
    }
}