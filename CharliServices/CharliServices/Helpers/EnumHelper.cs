using McpNetwork.Charli.Server.Models.Attributes;
using McpNetwork.Charli.Server.Models.Enums;
using System.Reflection;

namespace McpNetwork.Charli.Server.Helpers
{
    public static class EnumHelper
    {
        public static Type GetManagerImplementedType(this Enum value)
        {
            if (value.GetType().Name != typeof(EManagersType).Name)
            {
                return null;
            }

            MemberInfo memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return (memberInfo == null ? null : memberInfo.GetCustomAttribute(typeof(ManagerInterfaceAttribute)) as ManagerInterfaceAttribute)?.ImplementedInterface;
        }
    }
}
