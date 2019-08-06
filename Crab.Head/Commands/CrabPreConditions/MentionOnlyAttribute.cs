using Discord.Commands;

namespace Crab.Commands
{
    public class MentionOnlyAttribute : CrabPreconditionAttribute
    {
        public override PreconditionResult check(CommandContext context)
        {
            int argpos = 0; //it will complain else
            if(context.Message.HasMentionPrefix(context.Crab.CurrentUser, ref argpos))
                return PreconditionResult.FromSuccess();
            return PreconditionResult.FromFailure();
        }
    }
}