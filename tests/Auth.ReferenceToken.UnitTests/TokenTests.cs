using AspNetCore.Authentication.ReferenceToken;
using Xunit;
using Xunit.Abstractions;

namespace Auth.ReferenceToken.UnitTests
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
            var token = TokenTools.CreateToken("1");
            _outputHelper.WriteLine(token);
        }
    }
}
