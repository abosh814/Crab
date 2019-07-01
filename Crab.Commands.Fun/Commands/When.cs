using Discord.Commands;
using System.Threading.Tasks;

namespace Crab
{
    [LogModule]
    public class WhenModule : ModuleBase<SocketCommandContext>
    {
        [Command("when")]
        [RegexAlias("\\S\\s+when[\\s*?.!)]*$")]
        public Task when()
            => ReplyAsync("When you code it.");
    }
}