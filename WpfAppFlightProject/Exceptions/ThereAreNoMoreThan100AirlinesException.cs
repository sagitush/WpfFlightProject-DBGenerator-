using System;
using System.Runtime.Serialization;

namespace WpfAppFlightProject
{
    [Serializable]
    internal class ThereAreNoMoreThan100AirlinesException : Exception
    {
        public ThereAreNoMoreThan100AirlinesException()
        {
        }

        public ThereAreNoMoreThan100AirlinesException(string message) : base(message)
        {
        }

        public ThereAreNoMoreThan100AirlinesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ThereAreNoMoreThan100AirlinesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}