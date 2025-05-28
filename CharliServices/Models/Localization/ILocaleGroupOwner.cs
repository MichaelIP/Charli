namespace McpNetwork.Charli.Server.Models.Localization
{
    public interface ILocaleGroupOwner
    {
        string Name { get; set; }
        List<XmlGroups> Groups { get; set; }
        List<XmlLocalization> Localizations { get; set; }
    }
}
