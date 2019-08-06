namespace Crab.Commands
{
    public class DMResponseAttribute : ContextManipulatorAttribute
    {
        public override void modify(ref CommandContext context)
        {
            context.Channel = context.Invoker.GetOrCreateDMChannelAsync().GetAwaiter().GetResult();
        }
    }
}