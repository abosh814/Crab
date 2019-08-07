using System;

namespace Crab
{
    public class HasDataFileAttribute : Attribute
    {
        public string datafile;

        public HasDataFileAttribute(string filename)
        {
            datafile = filename;
        }
    }
}