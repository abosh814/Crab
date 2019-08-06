using System;

namespace Crab.Commands
{
    public abstract class CrabPreconditionAttribute : Attribute
    {
        public abstract PreconditionResult check(CommandContext context);
    }
}