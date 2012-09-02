namespace Hircine.Core.Connectivity
{
    /// <summary>
    /// Helper class used for building connection strings from raw parameters (Url, User, Password, etc...)
    /// 
    /// Wait, why does this exist when we can just assign those properties onto a DocumentStore instance directly? Glad you asked!
    /// Reason is - the RavenDbConnectionStringParser has some connection string validation rules built-into it that we don't want 
    /// to replicate, so this class allows us to take advantage of those same rules.
    /// </summary>
    public static class RavenConnectionStringBuilder
    {
        public const string UrlConnectionString = @"Url={0}";
        public const string UrlAndDefaultDatabaseConnectionString = @"Url={0};Database={1}";
        public const string UrlAndApiKeyConnectionString = @"Url={0};ApiKey={1}";
        public const string UrlAndCredentialsConnectionString = @"Url={0};User={1};Password={2}";
        public const string UrlAndCredentialsAndDefaultDatabaseConnectionString = @"Url={0};User={1};Password={2};Database={3}";

        public static string BuildConnectionString(string url)
        {
            return string.Format(UrlConnectionString, url);
        }

        public static string BuildConnectionString(string url, string defaultDb)
        {
            return string.Format(UrlAndDefaultDatabaseConnectionString, url, defaultDb);
        }

        public static string BuildConnectionStringWithApiKey(string url, string apiKey)
        {
            return string.Format(UrlAndApiKeyConnectionString, url, apiKey);
        }

        public static string BuildConnectionString(string url, string user, string password)
        {
            return string.Format(UrlAndCredentialsConnectionString, url, user, password);
        }

        public static string BuildConnectionString(string url, string user, string password, string defaultDb)
        {
            return string.Format(UrlAndCredentialsAndDefaultDatabaseConnectionString, url, user, password, defaultDb);
        }
    }
}
