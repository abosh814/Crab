namespace Crab.Commands
{
    public class AdminOnlyAttribute : CrabPreconditionAttribute
    {
        public override PreconditionResult check(CommandContext context)
        {
            if(Utils.isadmin(context.Invoker.Id))
                return PreconditionResult.FromSuccess();
            return PreconditionResult.FromFailure("You need to be an admin to execute this command");
        }
    }
}