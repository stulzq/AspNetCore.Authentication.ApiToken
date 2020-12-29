# AspNetCore.Authentication.ApiToken

中文 | [English](README.md)

[![Latest version](https://img.shields.io/nuget/v/AspNetCore.Authentication.ApiToken.svg)](https://www.nuget.org/packages/AspNetCore.Authentication.ApiToken/) 

AspNetCore.Authentication.ApiToken 是一个用于 ASP.NET Core 的认证组件，遵循 ASP.NET Core 的认证框架设计规范。它主要用于 WebApi 项目，提供**签发**和**校验** Token 的能力。本组件签发的 Token 非 Json Web Token(JWT)，类似于 IdentityServer4 中的 Reference Token，需要在服务端查询来进行有效性的验证。如果在你的项目中有 IdentityServer4 中的 Reference Token 需求，那么在中大型项目中推荐使用 IdentityServer4，如果是中小型项目，那么你可以考虑 AspNetCore.Authentication.ApiToken，它比 IdentityServer4 更加的轻便，接入和维护成本更低。此 Token 比 JWT 带来的优势是可以完全控制 Token 的生命周期，缺点是验证 Token 需要每次查询存储来比对验证（可以通过缓存来提升性能）。

## 功能

- 接入简单，只需要实现两个接口
- 一体化签发、刷新、注销和验证 Token
- 支持缓存，默认已实现 Redis，可轻松扩展其它缓存
- 支持定期清理过期 Token 后台任务
- 支持更新用户 Claim （角色）立即生效，无需重新登录
- 支持同一用户同一时间只能有一个 Token 生效（签发新Token，所有旧 Token 都会失效）
- 支持刷新 Token 时平滑过渡，旧Token不会立即失效
- 支持认证事件


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
        var user = await _dbContext.Users.FirstAsync(a => a.Id == userId);
        return new List<Claim>()
        {
            new Claim(ApiTokenClaimTypes.Subject,userId),
            new Claim(ApiTokenClaimTypes.Name,user.Name),
            new Claim(ApiTokenClaimTypes.Role,user.Role),
        };
    }
}
````

### 3.实现接口 IApiTokenStore

此接口用于存储和查询、删除 Token。因为本组件提供的 Token 需要查询比对进行有效性验证。

示例以数据库作为存储实现（Entity Framework core）：

MqApiTokenStore.cs
````csharp
public class MqApiTokenStore : IApiTokenStore
{
    //存储Token
    public async Task StoreAsync(TokenModel token)
    {
        //...
    }

    //存储Token列表
    public async Task StoreAsync(List<TokenModel> token)
    {
        //...
    }

    //获取Token
    public async Task<TokenModel> GetAsync(string token, string scheme)
    {
        //...
    }

    //获取Token列表
    public async Task<List<TokenModel>> GetListAsync(string userId, string scheme)
    {
        //...
    }

    //获取指定类型的Token列表
    public async Task<List<TokenModel>> GetListAsync(string userId, string scheme, TokenType type)
    {
        //...
    }

    //更新
    public async Task UpdateAsync(TokenModel token)
    {
        //...
    }

    //更新列表
    public async Task UpdateListAsync(List<TokenModel> token)
    {
        //...
    }

    //删除
    public async Task RemoveAsync(string token, string scheme)
    {
        //...
    }

    //删除列表
    public async Task RemoveListAsync(string userId, string scheme)
    {
        //...
    }

    //删除指定类型的Token列表
    public async Task RemoveListAsync(string userId, string scheme, TokenType type)
    {
        //...
    }

    //删除过期Token
    public async Task<int> RemoveExpirationAsync()
    {
        //...
    }
}
````

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

返回的结果中包含了 Bearer Token 和 Refresh Token。Bearer Token 用于接口验证，Refresh Token 用于 Token 的刷新。

### 6.使用 Token

类似于JWT的使用方式，在请求中加入 Header

````
Authorization: Bearer <token>
````

### 7.Demo

**完整的实现请参阅 [SampleApp](./sample/AspNetCore.ApiToken.SampleApp/README.md)**

![](assets/op.gif)

## 进阶

### 1.使用缓存

安装包：`AspNetCore.Authentication.ApiToken.Redis`

在注册服务中添加 `AddRedisCache(op => op.ConnectionString = "<redis连接字符串>")`

示例：

```csharp
services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
    .AddApiToken(op => op.UseCache = false)
    .AddRedisCache(op => op.ConnectionString = "127.0.0.1:6379")
    .AddProfileService<MyApiTokenProfileService>()
    .AddTokenStore<MyApiTokenStore>();
```

可以自定义缓存有效期，一般缓存有效期与token过期时间相同

### 2.实现自定义缓存

实现 `IApiTokenCacheService` 接口，可以参考 [Redis](src/AspNetCore.AuthenticationApiToken.Redis/RedisTokenCacheService.cs) 的实现。

### 3.定期清理 Token 服务

定期清理服务指在固定的时间间隔运行清理数据库中已过期的Token，在注册服务中添加 `AddCleanService()`

示例：

````csharp
services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
    .AddApiToken(op => op.UseCache = false)
    .AddProfileService<MyApiTokenProfileService>()
    .AddTokenStore<MyApiTokenStore>()
    .AddCleanService();
````

可以自定义间隔时间

### 4.使用刷新token

注入 `IApiTokenOperator ` 并调用 `RefreshAsync(string refreshToken, string scheme)`方法即可，会自动刷新并返回结果。

`ApiTokenOptions.KeepTokenValidTimeSpanOnRefresh` 属性可以设置刷新后，旧 Token 仍可以生效多久。

### 5.更新 claim

注入 `IApiTokenOperator ` 并调用 `RefreshClaimsAsync(string token, string scheme)` 方法即可。主要用于用户更新了资料，比如姓名或者角色，如果不需要重新登录，立即生效可以调用此方法。

### 6.注销token

注入 `IApiTokenOperator ` 并调用 `RemoveAsync(string token, string scheme)` 方法即可。

### 提示

以上方法中的 scheme 可不传，但是在注册了多个 ApiToken 认证服务，或者是 ApiToken 认证不是默认 scheme 的情况下，需要传入。这是因为 ASP.NET Core 的认证框架设计，需要了解详情的可以去看 ASP.NET Core官方文档。

## 感谢

本项目在设计和编写时参考了以下项目：

- [aspnetcore-authentication-apikey](https://github.com/mihirdilip/aspnetcore-authentication-apikey)
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/dotnet/aspnetcore/tree/master/src/Security/Authentication/JwtBearer/src) 
- [IdentityServer4](https://github.com/identityserver/identityserver4)



