namespace Crab.Commands
{
    public class PreconditionResult
    {
        public static PreconditionResult FromFailure()
            => new PreconditionResult(false, "");

        public static PreconditionResult FromFailure(string message)
            => new PreconditionResult(false, message);

        public static PreconditionResult FromSuccess()
            => new PreconditionResult(true, "");

        public static PreconditionResult FromSuccess(string message)
            => new PreconditionResult(true, message);

        public readonly bool Success;

        public readonly string Message;

        public PreconditionResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}