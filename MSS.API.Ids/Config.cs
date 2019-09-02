using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace MSS.API.Ids
{
    public class Config
    {

        public static IEnumerable<ApiResource> GetApiResources() //api
        {
            return new List<ApiResource>(){
                new ApiResource("MssService","MSS.API.Core")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients(int expires_in)
        {
            return new List<Client>
            {

                new Client(){
                    ClientId="pwdClient",
                    //OAuth密码模式
                     AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                     ClientSecrets={new Secret("secret".Sha256())},
                     RefreshTokenUsage = TokenUsage.ReUse,
                     RefreshTokenExpiration = TokenExpiration.Absolute,
                     //AlwaysIncludeUserClaimsInIdToken = true,
                     AccessTokenLifetime = expires_in,
                     AllowOfflineAccess = true,
                     //AbsoluteRefreshTokenLifetime = 1200,
                     AllowedScopes={ "MssService",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess}
                },


            };
        }

        public static List<TestUser> GetTestUser()
        {
            return new List<TestUser>() {
                new TestUser(){
                    SubjectId = "1",
                    Username ="zps",
                    Password = "zps",
                    Claims = new List<Claim>(){
                        new Claim("role","zps"),
                        new Claim("aaa","asdasdsd"),
                        new Claim(JwtClaimTypes.Role,"superadmin"),
                    }
                },
                 new TestUser(){
                    SubjectId = "2",
                    Username ="admin",
                    Password = "admin",
                     Claims = new List<Claim>(){
                        new Claim("role","admin")
                    }
                }
            };
        }

    }
}
