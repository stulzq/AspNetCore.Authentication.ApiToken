# AspNetCore.Authentication.ApiToken

English | [中文](README_zh-CN.md)

[![Latest version](https://img.shields.io/nuget/v/AspNetCore.Authentication.ApiToken.svg)](https://www.nuget.org/packages/AspNetCore.Authentication.ApiToken/) 

AspNetCore.Authentication.ApiToken is an authentication component for ASP.NET Core, following the design specification of ASP.NET Core authentication framework. It is mainly used in the WebApi project to provide **issuance** and **verification** Token capabilities. The Token issued by this component is not a Json Web Token (JWT), which is similar to the Reference Token in IdentityServer4 and needs to be queried on the server to verify the validity. If there is a need for Reference Token in IdentityServer4 in your project, then IdentityServer4 is recommended for medium and large projects. If it is a small and medium-sized project, then you can consider AspNetCore.Authentication.ApiToken, which is more portable than IdentityServer4. Maintenance costs are lower. The advantage of this Token over JWT is that it can completely control the life cycle of the Token. The disadvantage is that to verify the Token, you need to query the storage every time to compare and verify (the performance can be improved by caching).

## Features

- Simple access, only need to implement two interfaces
- Integrated issuance, refresh, cancellation and verification of Token
- Support caching, Redis is implemented by default, and other caches can be easily extended
- Support regular cleaning of expired Token background tasks
- Support to update the user claim (role) to take effect immediately without logging in again
- Only one Token can be valid for the same user at the same time (if a new Token is issued, all old Tokens will become invalid)
- Support smooth transition when refreshing Token, old Token will not be invalid immediately
- Support authentication events


## Quick start

### 1.Install

Install via Nuget in your WebApi project

````shell
dotnet add package AspNetCore.Authentication.ApiToken
````

### 2.Implementation interface IApiTokenProfileService

The main function of this interface is to query the user's Claims according to the user Id when **creating** and **refreshing** Tokens, such as commonly used: Name, Id, and Role.

The Claims provided here can be accessed in the `HttpContext.User.Claims` property after **authentication is successful**. Role Claim can be used on `[Authorize]`, such as `[Authorize(Roles = "Admin")]`

Example（Entity Framework core）：

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

### 3.Implementation interface IApiTokenStore

This interface is used to store, query, and delete tokens. Because the Token provided by this component needs to be checked and compared for validity verification.

The example uses the database as a storage implementation (Entity Framework core):

MqApiTokenStore.cs
````csharp
public class MqApiTokenStore : IApiTokenStore
{
    //Store token
    public async Task StoreAsync(TokenModel token)
    {
        //...
    }

    //Store Token list
    public async Task StoreAsync(List<TokenModel> token)
    {
        //...
    }

    //Get token
    public async Task<TokenModel> GetAsync(string token, string scheme)
    {
        //...
    }

    //Get the token list
    public async Task<List<TokenModel>> GetListAsync(string userId, string scheme)
    {
        //...
    }

    //Get a list of tokens of the specified type
    public async Task<List<TokenModel>> GetListAsync(string userId, string scheme, TokenType type)
    {
        //...
    }

    //Update token
    public async Task UpdateAsync(TokenModel token)
    {
        //...
    }

    //Update token list
    public async Task UpdateListAsync(List<TokenModel> token)
    {
        //...
    }

    //Delete token
    public async Task RemoveAsync(string token, string scheme)
    {
        //...
    }

    //Delete list
    public async Task RemoveListAsync(string userId, string scheme)
    {
        //...
    }

    //Delete the Token list of the specified type
    public async Task RemoveListAsync(string userId, string scheme, TokenType type)
    {
        //...
    }

    //Remove expiration token
    public async Task<int> RemoveExpirationAsync()
    {
        //...
    }
}
````

### 4.Configuration

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

### 5.Issue token

You need to write an API for issuing tokens yourself.

Inject `IApiTokenOperator tokenOperator`

````csharp
var createResult = await tokenOperator.CreateAsync("<userId>");
````

The returned result contains Bearer Token and Refresh Token. Bearer Token is used for interface verification, and Refresh Token is used for Token refresh.

### 6.Use Token

Similar to the way of using JWT, add Header to the request

````
Authorization: Bearer <token>
````

### 7.Demo

**Please refer to the complete implementation [SampleApp](./sample/AspNetCore.ApiToken.SampleApp/README.md)**

![](assets/op.gif)

## Advance

### 1.Use cache

Install Nuget package：`AspNetCore.Authentication.ApiToken.Redis`

Add service on Startup.ConfigureServices `AddRedisCache(op => op.ConnectionString = "<redis connection string>")`

Example：

```csharp
services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
    .AddApiToken(op => op.UseCache = false)
    .AddRedisCache(op => op.ConnectionString = "127.0.0.1:6379")
    .AddProfileService<MyApiTokenProfileService>()
    .AddTokenStore<MyApiTokenStore>();
```

The cache validity period can be customized, generally the cache validity period is the same as the token expiration time.

### 2.Custom cache

To implement the `IApiTokenCacheService` interface, please refer to the implementation of [Redis](src/AspNetCore.AuthenticationApiToken.Redis/RedisTokenCacheService.cs).

### 3.Clean Token Background Service

Periodic cleaning service refers to running to clean up expired tokens in the database at regular intervals, adding `AddCleanService()` to the registration service

Example:

````csharp
services.AddAuthentication(ApiTokenDefaults.AuthenticationScheme)
    .AddApiToken(op => op.UseCache = false)
    .AddProfileService<MyApiTokenProfileService>()
    .AddTokenStore<MyApiTokenStore>()
    .AddCleanService();
````

Can customize the interval time.

### 4.Refresh Token

Inject `IApiTokenOperator` and call the `RefreshAsync(string refreshToken, string scheme)` method, it will automatically refresh and return the result.

The `ApiTokenOptions.KeepTokenValidTimeSpanOnRefresh` property can be used to set how long the old Token can be valid after refreshing.

### 5.Update claim

Inject `IApiTokenOperator` and call `RefreshClaimsAsync(string token, string scheme)` method. Mainly used for users to update information, such as name or role, if you do not need to login again, it will take effect immediately, you can call this method.

### 6.Revoke token

Inject `IApiTokenOperator` and call `RemoveAsync(string token, string scheme)` method.

### Tips

The scheme in the above method can not be passed, but it needs to be passed in when multiple ApiToken authentication services are registered, or the ApiToken authentication is not the default scheme. This is because of the design of the authentication framework of ASP.NET Core. If you need to know the details, you can see the official documentation of ASP.NET Core.

## Thanks

The following items are referred to in the design and compilation of this project：

- [aspnetcore-authentication-apikey](https://github.com/mihirdilip/aspnetcore-authentication-apikey)
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/dotnet/aspnetcore/tree/master/src/Security/Authentication/JwtBearer/src) 
- [IdentityServer4](https://github.com/identityserver/identityserver4)



