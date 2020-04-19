using System;
using System.Runtime.Serialization;

namespace StoryBuckets.Integrations.CsvIntegration
{
    [Serializable]
    public class UnexpectedCsvHeaderException : Exception
    {
        public UnexpectedCsvHeaderException()
        {
        }

        public UnexpectedCsvHeaderException(string message) : base(message)
        {
        }

        public UnexpectedCsvHeaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedCsvHeaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}