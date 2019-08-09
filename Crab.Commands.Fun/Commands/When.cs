using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crab.Commands.Fun
{
    [LogModule]
    public class WhenModule : CrabCommandModule
    {
        [CrabCommand("\\S\\s+when[\\s*?.!)]*$")]
        public static Task when(Match m, CommandContext context)
            => context.Channel.SendMessageAsync("When you code it.");
    }
}