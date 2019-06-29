using System;

namespace Crab
{
    public abstract class CrabCore //only one class should inherit this
    {
        public static void loaded(){}
    }

    public class LogModule : Attribute
    {

    }
}