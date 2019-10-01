using System;
using System.Runtime.Serialization;

namespace WpfAppFlightProject
{
    [Serializable]
    internal class YouNeedToPutNumberOfAirlinesException : Exception
    {
        public YouNeedToPutNumberOfAirlinesException()
        {
        }

        public YouNeedToPutNumberOfAirlinesException(string message) : base(message)
        {
        }

        public YouNeedToPutNumberOfAirlinesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected YouNeedToPutNumberOfAirlinesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}