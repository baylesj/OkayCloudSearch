namespace OkayCloudSearch
{
    public static class Constants
    {
        public enum Operators
        {
            And = 0,
            Or,
            Not 
        }
    }

    public static class Helpers
    {
        public static string ToQueryString(this Constants.Operators k)
        {
            // Lucene constants must be upper case
            return " " + k.ToString().ToUpperInvariant() + " ";
        }
    }
}
