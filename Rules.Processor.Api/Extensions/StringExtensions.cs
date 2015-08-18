namespace Rules.Processor.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceQuotesForSpecialChars(this string str)
        {
            if (str[0] != '"' || str[str.Length - 1] != '"')
                return str;
            str = str.Substring(1);
            str = str.Substring(0, str.Length - 1);
            return str;
        }
    }
}
