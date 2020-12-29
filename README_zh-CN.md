# AspNetCore.Authentication.ApiToken

[![Latest version](https://img.shields.io/nuget/v/AspNetCore.Authentication.ApiToken.svg)](https://www.nuget.org/packages/AspNetCore.Authentication.ApiToken/) 

AspNetCore.Authentication.ApiToken 是一个用于 ASP.NET Core 的认证组件，遵循 ASP.NET Core 的认证框架设计规范。它主要用于 WebApi 项目，提供**签发**和**校验** Token 的能力。本组件签发的 Token 非 Json Web Token(JWT)，类似于 IdentityServer4 中的 Reference Token，需要在服务端查询来进行有效性的验证。如果在你的项目中有 IdentityServer4 中的 Reference Token 需求，那么在中大型项目中推荐使用 IdentityServer4，如果是中小型项目，那么你可以考虑 AspNetCore.Authentication.ApiToken，它比 IdentityServer4 更加的轻便，接入和维护成本更低。此 Token 比 JWT 带来的优势是可以完全控制 Token 的生命周期，缺点是验证 Token 需要每次查询存储来比对验证（可以通过缓存来提升性能）。

## 功能

- 接入简单，只需要实现两个接口
- 一体化签发、刷新、注销和验证 Token
- 支持缓存，默认已实现 Redis，可轻松扩展其它缓存
- 支持定期清理过期 Token 后台任务
- 支持更新用户 Claim （角色）立即生效，无需重新登录
- 认证事件


## 快速入门

### 1.安装

在你的 WebApi 项目中通过 Nuget 安装

````shell
dotnet add package AspNetCore.Authentication.ApiToken
````

### 2.实现接口 IApiTokenProfileService

此接口的主要功能为，在**创建**和**刷新** Token 时，根据用户Id查询用户的 Claims，如常用的：Name、Id、Role。

这里提供的 Claims，将可以在**认证成功**后的 `HttpContext.User.Claims` 属性中被访问。Role Claim 将可以用在 `[Authorize]`上，如 `[Authorize(Roles = "Admin")]`

示例（Entity Framework core）：

MyApiTokenProfileService.cs

````csharp
public class MyApiTokenProfileService : IApiTokenProfileService
{
    private readonly EfDbContext _dbContext;

    public MyApiTokenProfileService(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Claim>> GetUserClaimsAsync(string userId)
    {
        var user = await _dbContext.Users.Where(a => a.Id == userId).FirstAsync();
        var result=new List<Claim>(3);

        result.Add(new Claim(ApiTokenClaimTypes.Subject, "1"));
        result.Add(new Claim(ApiTokenClaimTypes.Name, "Alex"));
        result.Add(new Claim(ApiTokenClaimTypes.Role, "Admin"));

        return result;
    }
}
````

### 3.实现接口 IApiTokenStore

此接口用于存储和查询、删除 Token。因为本组件提供的 Token 需要查询比对进行有效性验证。

示例以数据库作为存储实现（Entity Framework core）：

ApiToken.cs

````csharp
[Table("ApiToken")]
public class ApiToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    //Token的值
    [Required]
    public string Token { get; set; }

    //Token类型 Bearer、Refresh
    [Required]
    public string Type { get; set; }

    //用户Id
    [Required]
    public string UserId { get; set; }

    //用户 Claims （Json）
    [Required]
    public string Claims { get; set; }

    //创建时间
    [Required]
    public DateTime CreateTime { get; set; }

    //过期时间
    [Required]
    public DateTime Expiration { get; set; }
}
````

MqApiTokenStore.cs
````csharp
public class MqApiTokenStore : IApiTokenStore
{
    private readonly EfDbContext _dbContext;

    public MqApiTokenStore(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // 存储 Token
    public async Task StoreAsync(TokenModel token)
    {
        var entity = ConvertToApiTokenEntity(token);
        await _dbContext.ApiToken.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    // 存储Token列表
    public async Task StoreAsync(List<TokenModel> token)
    {
        var entities = token.Select(ConvertToApiTokenEntity).ToList();
        await _dbContext.ApiToken.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }

    //获取Token
    public async Task<TokenModel> GetAsync(string token)
    {
        var entity = await _dbContext.ApiToken.Where(a => a.Token == token).FirstOrDefaultAsync();
        return entity == null ? null : ConvertToTokenModel(entity);
    }

    //获取Token列表
    public async Task<List<TokenModel>> GetListAsync(string userId)
    {
        var queryResult = await _dbContext.ApiToken.Where(a => a.UserId == userId).ToListAsync();
        var result = queryResult.Select(ConvertToTokenModel).ToList();
        return result;
    }

    //更新指定 Token 的 Claims，用于用户更改了姓名、角色等Claim，立即生效
    public async Task UpdateClaimsAsync(string token, IReadOnlyList<Claim> claims)
    {
        var tokenEntity = await _dbContext.ApiToken.Where(a => a.Token == token).FirstAsync();
        tokenEntity.Claims = JsonConvert.SerializeObject(claims);
        _dbContext.ApiToken.Update(tokenEntity);
        await _dbContext.SaveChangesAsync();
    }

    //移除Token
    public async Task RemoveAsync(string token)
    {
        var tokenEntity = await _dbContext.ApiToken.Where(a => a.Token == token).FirstOrDefaultAsync();
        if (tokenEntity != null)
        {
            _dbContext.ApiToken.Remove(tokenEntity);
            await _dbContext.SaveChangesAsync();
        }
    }

    //移除Token列表
    public async Task RemoveListAsync(string userId)
    {
        var tokens = _dbContext.ApiToken.Where(a => a.UserId == userId);
        _dbContext.ApiToken.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync();
    }

    //移除已过期Token
    public async Task<int> RemoveExpirationAsync()
    {
        var tokens = _dbContext.ApiToken.Where(a => a.Expiration < DateTime.Now);
        var count = await tokens.CountAsync();
        _dbContext.ApiToken.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync();
        return count;
    }

    //将 ApiToken 转换为 TokenModel
    private TokenModel ConvertToTokenModel(ApiToken apiToken)
    {
        var result = new TokenModel()
        {
            CreateTime = apiToken.CreateTime,
            Expiration = apiToken.Expiration,
            Type = Enum.Parse<TokenType>(apiToken.Type),
            UserId = apiToken.UserId,
            Value = apiToken.Token
        };

        return result;

    }

    //将 TokenModel 转换为 ApiToken
    private ApiToken ConvertToApiTokenEntity(TokenModel tokenModel)
    {
        var result = new ApiToken()
        {
            CreateTime = tokenModel.CreateTime.DateTime,
            Expiration = tokenModel.Expiration.DateTime,
            Type = tokenModel.Type.ToString(),
            UserId = tokenModel.UserId,
            Token = tokenModel.Value,
        };

        if (tokenModel.Claims != null)
        {
            result.Claims = JsonConvert.SerializeObject(ConvertToClaimStoreModel(tokenModel.Claims));
        }
        else
        {
            result.Claims = "[]";
        }

        return result;
    }

    //Claim 在反序列化的时候会失败，所以此处定义的单独类型用于序列化和反序列化 
    //你也可以像 Identity 一样，将用户 Claim 存储在单独表中，就不用序列化了，这里直接序列化是为了方便
    public List<ClaimStoreModel> ConvertToClaimStoreModel(IReadOnlyList<Claim> claims)
    {
        return claims.Select(a => new ClaimStoreModel() { Type = a.Type, Value = a.Value, }).ToList();
    }

    public List<Claim> ConvertToClaim(List<ClaimStoreModel> claims)
    {
        return claims.Select(a => new Claim(a.Type, a.Value)).ToList();
    }

    public class ClaimStoreModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
````

> 以上示例实现，仅供**参考**

### 4.配置

Startup.cs

````csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
        .AddApiToken()
        .AddProfileService<MyApiTokenProfileService>()
        .AddTokenStore<MyApiTokenStore>();
    //Other services...
}
````

### 5.签发Token

你需要自己编写一个签发 Token 的 Api

注入 `IApiTokenOperator tokenOperator`

调用

````csharp
var createResult = await tokenOperator.CreateAsync("<用户Id>");
````

返回的结果中包含了 Bearer Token 和 Refresh Token。Bearer Token 用于接口验证，Refresh Token 用于 Token 的刷新

## 进阶



## 感谢

本项目在设计和编写时参考了以下项目：

- [aspnetcore-authentication-apikey](https://github.com/mihirdilip/aspnetcore-authentication-apikey)
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/dotnet/aspnetcore/tree/master/src/Security/Authentication/JwtBearer/src) 



