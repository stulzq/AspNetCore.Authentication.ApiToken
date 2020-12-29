namespace AspNetCore.ApiToken.SampleApp.Store
{
    /// <summary>
    /// From https://github.com/IdentityServer/IdentityServer4
    /// </summary>
    public class ClaimLite
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }

    }
}