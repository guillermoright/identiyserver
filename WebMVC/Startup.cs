using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Services;
using WebMVC.ViewModel;

namespace WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession();
            services.AddControllers();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            IdentityModelEventSource.ShowPII = true;

            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                //options.DefaultChallengeScheme = "oidc";
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

            })
                .AddCookie("Cookies")
                
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "http://localhost:5000";
                    options.SignedOutRedirectUri = "http://localhost:28880/";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    //options.UsePkce = true;
                    options.SaveTokens = true;
                    options.Scope.Add("profile");
                    options.Scope.Add("openid");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.CallbackPath = "/signin-oidc";
                    options.Scope.Add("openid");
                    //options.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clientpassword"))
                    //};
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
