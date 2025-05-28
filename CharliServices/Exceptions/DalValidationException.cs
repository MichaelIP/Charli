namespace McpNetwork.Charli.Server.Exceptions
{
    public class DalValidationException : Exception
    {
        public DalValidationException() { }
        public DalValidationException(string message) : base(message) { }
        public DalValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
