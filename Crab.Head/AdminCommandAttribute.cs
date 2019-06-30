using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace Crab
{
    public class AdminCommand : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(Utils.isadmin(context.User.Id))
                return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError("You need to be an admin to execute this command");
        }
    }
}