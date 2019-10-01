using System;
using System.Runtime.Serialization;

namespace WpfAppFlightProject
{
    [Serializable]
    internal class YouNeedToHaveFlightsAndCustomersToGetTicketsException : Exception
    {
        public YouNeedToHaveFlightsAndCustomersToGetTicketsException()
        {
        }

        public YouNeedToHaveFlightsAndCustomersToGetTicketsException(string message) : base(message)
        {
        }

        public YouNeedToHaveFlightsAndCustomersToGetTicketsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected YouNeedToHaveFlightsAndCustomersToGetTicketsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}