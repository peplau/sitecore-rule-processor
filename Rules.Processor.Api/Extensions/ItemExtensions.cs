namespace Rules.Processor.Api.Extensions
{
    public static class ItemExtensions
    {
        public static string GetItemXPath(this Sitecore.Data.Items.Item item)
        {
            return (item.Paths.Path.Replace("/", "#/#") + "#").Substring(1);
        }
    }
}
