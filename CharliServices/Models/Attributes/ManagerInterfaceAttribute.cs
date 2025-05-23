namespace McpNetwork.Charli.Server.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ManagerInterfaceAttribute(Type implementedInterface) : Attribute
    {
        public Type ImplementedInterface { get; internal set; } = implementedInterface;
    }
}
