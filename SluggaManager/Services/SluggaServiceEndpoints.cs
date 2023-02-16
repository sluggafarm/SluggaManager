namespace SluggaManager.Services
{
    internal static class SluggaServiceEndpoints
    {
        internal static class V1
        {
            public const string BaseUri = @"https://pastelworld.io/slugga-api/api";
            public const string GetSluggaState = @"{baseUri}/{version}/slug/{id}/{wallet}";
            public const string GetShardAmount = @"{baseUri}/{version}/shards/{wallet}";
            
            public const string SluggaSleepAction = @"{baseUri}/{version}/slug/sleep/{id}/{wallet}";
            public const string SluggaPetAction = @"{baseUri}/{version}/slug/pet/{id}/{wallet}";
            public const string SluggaFeedAction = @"{baseUri}/{version}/slug/feed/{id}/{wallet}";

            public static string ParseUrl(string url, string wallet, string id = "")
            {
                return url.Replace("{baseUri}", BaseUri)
                            .Replace("{wallet}", wallet)
                            .Replace("{id}", id)
                            .Replace("{version}", "v1");
            }
        }
    }
}