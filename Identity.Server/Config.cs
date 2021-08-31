using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Server
{
    public class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API"),
                new ApiScope("api", " API")
            };


        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                   new IdentityResource("roles", new[] { JwtClaimTypes.Role })
                {
                    Required = true
                }
            };

        public static IEnumerable<Client> Clients =>
     new List<Client>
     {
        // machine to machine client (from quickstart 1)
        new Client
        {
            ClientId = "client",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.ClientCredentials,
            // scopes that client has access to
            AllowedScopes = { "api1" },
            AccessTokenLifetime = 1
        },
        // interactive ASP.NET Core MVC client
        new Client
        {
            ClientId = "mvc",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris = { "http://localhost:28880/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "http://localhost:28880/signout-callback-oidc" },
            //RequirePkce = true,
            //        AllowPlainTextPkce = false,
            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile

            }
        },
         new Client
                {
                    ClientId = "js",
                    ClientName = "Harria Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =           { $"http://localhost:15000/oidc-client-sample.html" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"http://localhost:15000/" },
                    AllowedCorsOrigins =     { $"http://localhost:15000" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                         "api"

                    },
                },
     };

    }
}