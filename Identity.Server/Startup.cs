using Identity.Server.Data;
using Identity.Server.Model;
using Identity.Server.Services;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Identity.Server
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    }));

            //services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

          services.AddIdentityServer()
                    //.AddInMemoryIdentityResources(Config.IdentityResources)
                    //.AddInMemoryApiScopes(Config.ApiScopes)
                    //.AddInMemoryClients(Config.Clients)
                    //.AddTestUsers(TestUsers.Users)
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddConfigurationStore(options =>
                      {
                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(migrationsAssembly);
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(migrationsAssembly);
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    }).Services.AddTransient<IProfileService, ProfileService>();


            services.AddTransient<ILoginService<ApplicationUser>, LoginService>();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            IdentityModelEventSource.ShowPII = true;
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
            

            services.AddAuthentication()
                    .AddCookie()
                    .AddOpenIdConnect("aad", "Login with Azure AD", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                // "https://login.microsoftonline.com/fbf70b36-9f4a-4882-a409-cf8365dee9e5/v2.0/";  //
                options.Authority = "https://login.windows.net/fbf70b36-9f4a-4882-a409-cf8365dee9e5";   //$"https://login.microsoftonline.com/common";
                options.TokenValidationParameters =
                         new TokenValidationParameters { ValidateIssuer = false };
                options.ClientId = "645f0ce2-9562-4c5c-b049-7e3ce180fc92";
                //options.CallbackPath = "/signin-oidc";
                //options.CallbackPath = "/signin-oidc";


                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    NameClaimType = "name",
                //    RoleClaimType = "role"
                //};



            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MigrateMemoryDataToSqlServer(app);
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseStaticFiles();

            // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
            //    await next();
            //});
            
            app.UseForwardedHeaders();
            app.UseIdentityServer();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
           
            app.UseDeveloperExceptionPage();

            //app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax , Secure = CookieSecurePolicy.Always});
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                //endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                //{
                //    Predicate = _ => true,
                //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //});
                //endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                //{
                //    Predicate = r => r.Name.Contains("self")
                //});
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        private void MigrateMemoryDataToSqlServer(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var identity in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(identity.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var api in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(api.ToEntity());
                    }

                    context.SaveChanges();
                }
            }

        
        }
    
    }
}
