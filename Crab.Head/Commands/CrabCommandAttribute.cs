using System;
using System.Text.RegularExpressions;

namespace Crab.Commands
{
    public class CrabCommandAttribute : Attribute
    {
        public string pattern;
        public RegexOptions options;
        public CrabCommandAttribute(string regex, RegexOptions ops = RegexOptions.None)
        {
            pattern = regex;
            options = ops;
        }

    }
}