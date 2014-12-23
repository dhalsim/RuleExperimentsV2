using System;
using System.Runtime.Serialization;
using System.Text;

namespace Domain.RuleExperiments.Exceptions
{
    [Serializable]
    public class BaseAmadeusException : Exception, IAmadeusException
    {
        public BaseAmadeusException()
        {
        }

        public BaseAmadeusException(string message) : base(message)
        {
        }

        public BaseAmadeusException(string message, Exception exception) : base(message, exception)
        {
        }

        public BaseAmadeusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        protected internal virtual string GetExceptionLog()
        {
            StringBuilder sb = new StringBuilder();
            Exception exception = this;

            while (exception != null)
            {
                sb.AppendLine(exception.Message);
                sb.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            }

            return sb.ToString();
        }
    }
}