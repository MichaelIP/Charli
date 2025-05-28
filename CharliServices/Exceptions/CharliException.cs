namespace McpNetwork.Charli.Server.Exceptions
{
    internal class CharliException : Exception
    {
        public object Parameters { get; } = new object();

        public CharliException(string message) : base(message) { }

        public CharliException(string message, object parameters) : base(message) 
        {
            Parameters = parameters;
        }

        public CharliException(string message, Exception innerException) : base(message, innerException) { }

        public CharliException(string message, Exception innerException, object parameters) : base(message, innerException)
        {
            Parameters = parameters;
        }
    }
}
