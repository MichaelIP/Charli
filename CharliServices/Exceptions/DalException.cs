namespace McpNetwork.Charli.Server.Exceptions
{
    [Serializable]
    public class DalException : Exception
    {
        public DalException() { }
        public DalException(string message) : base(message) { }
        public DalException(string message, Exception inner) : base(message, inner) { }
    }
}
