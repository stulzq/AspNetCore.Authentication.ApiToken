using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.ApiToken.SampleApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly IApiTokenOperator _tokenOperator;

        public TokenController(ILogger<TokenController> logger, IApiTokenOperator tokenOperator)
        {
            _logger = logger;
            _tokenOperator = tokenOperator;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                new
                {
                    Token = await HttpContext.GetApiTokenAsync(),
                    User= HttpContext.User.Claims
                });
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> Create()
        {
            var createResult = await _tokenOperator.CreateAsync("1");
            return Ok(createResult);
        }
    }
}
