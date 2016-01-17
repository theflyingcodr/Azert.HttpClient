using System;

namespace Azert.HttpClient.Exceptions
{
    public class MissingHeaderException : Exception
    {
        public MissingHeaderException()
        {

        }

        public MissingHeaderException(string message) : base(message)
        {

        }
    }
}