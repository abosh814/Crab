using System;

namespace Crab.Commands
{
    public class CrabCommandAttribute : Attribute
    {
        public string pattern;
        public CrabCommandAttribute(string regex)
        {
            pattern = regex;
        }
    }
}