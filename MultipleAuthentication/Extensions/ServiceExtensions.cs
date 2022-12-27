using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MultipleAuthentication.Models;
using System.Net;
using System.Text;

namespace MultipleAuthentication.Extensions
{
    public static class ServiceExtensions
    {
        public static void BindConfiguration(this IServiceCollection services, IConfiguration config)
        {
            var authConfig = new AuthorizationConfig();
            config.Bind("Authorization", authConfig);
            services.AddSingleton(authConfig);
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddAuthentication()
                .AddCookieAuthentication()
                .AddJwtAuthentication()
                .AddAzureADAuthentication(config, env)
                .AddAWSAuthentication(config, env);
        }

        private static AuthenticationBuilder AddCookieAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddCookie(options =>
            {
                options.LoginPath = "/auth/unauthorized";
                options.AccessDeniedPath = "/auth/forbidden";
            });
        }

        private static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = "https://localhost:7208/",
                     ValidAudience = "https://localhost:7208/",
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@1"))
                 };
             });
        }

        private static AuthenticationBuilder AddAzureADAuthentication(this AuthenticationBuilder builder, IConfiguration config, IHostEnvironment env)
        {
            return builder.AddJwtBearer("AzureADScheme", opt =>
            {
                opt.Authority = config["Authorization:AzureAD:Issuer"];
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = config["Authorization:AzureAD:Audience"]
                };
                opt.Events = GetJwtBearerEvents(env);
            });
        }

        private static AuthenticationBuilder AddAWSAuthentication(this AuthenticationBuilder builder,
            IConfiguration config, IHostEnvironment env)
        {
            return builder.AddJwtBearer("AWSScheme", opt =>
            {
                opt.Authority = config["Authorization:AWS:Issuer"];
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, // Because audience claim doesn't exist in AWS access token
                    ValidAudience = config["Authorization:AWS:Audience"]
                };
                opt.Events = GetJwtBearerEvents(env);
            });
        }

        private static JwtBearerEvents GetJwtBearerEvents(IHostEnvironment env)
        {
            return new JwtBearerEvents
            {
                OnAuthenticationFailed = c =>
                {
                    c.NoResult();
                    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    c.Response.ContentType = "text/plain";
                    c.Response.WriteAsync(env.IsDevelopment()
                            ? c.Exception.ToString()
                            : string.Empty)
                        .Wait();
                    return Task.CompletedTask;
                }
            };
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddDefaultPolicy();
                options.AddCookiePolicy();
                options.AddAzureAdPolicy();
                options.AddAwsPolicy();
            });
        }

        private static void AddDefaultPolicy(this AuthorizationOptions options)
        {
            // Default policy is going to use all specified authentication schemes
            var defaultAuthPolicyBuilder = new AuthorizationPolicyBuilder(
                CookieAuthenticationDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme,
                "SecondJwtScheme");

            options.DefaultPolicy = defaultAuthPolicyBuilder.RequireAuthenticatedUser().Build();
        }

        private static void AddCookiePolicy(this AuthorizationOptions options)
        {
            var cookiePolicyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
            options.AddPolicy("OnlyCookieScheme", cookiePolicyBuilder
                .RequireAuthenticatedUser()
                .Build());
        }

        private static void AddAzureAdPolicy(this AuthorizationOptions options)
        {
            var azureAdPolicyBuilder = new AuthorizationPolicyBuilder("AzureADScheme");
            options.AddPolicy("OnlyAzureADUser", azureAdPolicyBuilder
                .RequireAuthenticatedUser()
                .Build());
        }

        private static void AddAwsPolicy(this AuthorizationOptions options)
        {
            var awsPolicyBuilder = new AuthorizationPolicyBuilder("AWSScheme");
            options.AddPolicy("OnlyAWSUser", awsPolicyBuilder
                .RequireAuthenticatedUser()
                .Build());
        }
    }
}
