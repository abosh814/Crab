using System;

namespace Crab.Commands
{
    public abstract class ContextManipulatorAttribute : Attribute
    {
        public abstract void modify(ref CommandContext context);
    }
}