using Discord;
using System;

namespace Crab.Commands
{
    public class CommandResult
    {
        public string message {get; private set;}

        public ExecutionResult result {get; private set;}

        public Emoji emote {get; private set;}

        public Exception exception {get; private set;}

        private CommandResult(PreconditionResult res) //only makes sense to use this for failures
        {
            result = ExecutionResult.FAILURE;

            if(res.respond)
            {
                message = res.Message;
                emote = new Emoji("❌");
            }
        }

        private CommandResult(ExecutionResult res)
        {
            result = res;
            if(res == ExecutionResult.SUCCESS)
                emote = new Emoji("✅");
        }

        private CommandResult(Exception e)
        {
            result = ExecutionResult.EXCEPTION;
            exception = e;
            emote = new Emoji("⚠");
        }

        public static CommandResult FromFailure(PreconditionResult res)
            => new CommandResult(res);

        public static CommandResult FromSuccess()
            => new CommandResult(ExecutionResult.SUCCESS);

        public static CommandResult FromException(Exception e)
            => new CommandResult(e);

        public static CommandResult FromNeutral()
            => new CommandResult(ExecutionResult.NEUTRAL);

        public bool isSmallerThan(CommandResult other)
        {
            return this.result < other.result;
        }

        public bool shouldExit()
        {
            return result > ExecutionResult.FAILURE;
        }
    }

    public enum ExecutionResult
    {
        NEUTRAL,
        FAILURE,
        SUCCESS,
        EXCEPTION
    }
}