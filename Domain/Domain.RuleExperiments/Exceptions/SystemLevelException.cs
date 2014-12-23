using System;
using System.Runtime.Serialization;

namespace Domain.RuleExperiments.Exceptions
{
    [Serializable]
    public class SystemLevelException : BaseAmadeusException
    {
        public SystemLevelException()
        {
        }

        public SystemLevelException(string message)
            : base(message)
        {
        }

        public SystemLevelException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public void Log()
        {
            throw new System.NotImplementedException();
        }

        public string GetGenericMessage()
        {
            throw new System.NotImplementedException();
        }

        public SystemLevelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}