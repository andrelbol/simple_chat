namespace SimpleChat.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string TrimNewLines(this string target)
            => target.TrimEnd('\r', '\n', '\0');
    }
}
