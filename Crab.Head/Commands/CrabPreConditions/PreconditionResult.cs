namespace Crab.Commands
{
    public class PreconditionResult
    {
        public static PreconditionResult FromFailure()
            => new PreconditionResult(false, null);

        public static PreconditionResult FromFailure(string message)
            => new PreconditionResult(false, message);

        public static PreconditionResult FromSuccess()
            => new PreconditionResult(true, null);

        public PreconditionResult NoResponse() //to prevent returning emote reactions on ping checks
        {
            respond = false;
            return this;
        }

        public readonly bool Success;

        public readonly string Message;

        public bool respond {get; private set;}

        public PreconditionResult(bool success, string message)
        {
            Success = success;
            Message = message;
            respond = true;
        }
    }
}