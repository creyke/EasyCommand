using System;

namespace EasyCommand.AspNetCore.Exceptions
{
    public class HandlerExecutionException : Exception
    {
        private static String message = "Failed to execute handler {0} running command ({1}). Failure occured when trying to run {2}, Please see inner exception for more details";
        public HandlerExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static HandlerExecutionException CreateWhenFailedBeforeCommandExecution(
            Type commandType, Type handlerType, Exception innerException)
        {
            return CreateWhenFailedCommandExecution("before", commandType, handlerType, innerException);
        }

        public static HandlerExecutionException CreateWhenFailedAfterCommandExecution(
            Type commandType, Type handlerType, Exception innerException)
        {
            return CreateWhenFailedCommandExecution("after", commandType, handlerType, innerException);
        }
        private static HandlerExecutionException CreateWhenFailedCommandExecution(
         string occurance, Type commandType, Type handlerType, Exception innerException)
        {
            return new HandlerExecutionException(
               string.Format(message, occurance, commandType.Name, handlerType.Name), innerException);
        }
    }
}
