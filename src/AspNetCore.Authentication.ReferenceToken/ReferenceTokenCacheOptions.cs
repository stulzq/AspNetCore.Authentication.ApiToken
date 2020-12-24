namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenCacheOptions
    {
        public string ConnectionString { get; set; }

        public string CachePrefix { get; set; } = "aspnetcore:auth:ref:{0}";
    }
}