using AspNetCore.Authentication.ApiToken;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCore.ApiToken.UnitTests
{
    public class TokenTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TokenTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void CreateToken()
        {
            var token = ApiTokenTools.CreateToken("1");
            Assert.Equal(64, token.Length);
            _outputHelper.WriteLine(token);
        }
    }
}
