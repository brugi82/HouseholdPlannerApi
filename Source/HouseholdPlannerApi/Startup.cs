using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseholdPlanner.Models.Options;
using HouseholdPlannerApi.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HouseholdPlannerApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var sendGridApiKey = Environment.GetEnvironmentVariable("HouseholdPlannerApiKeySendgrid");
            var key = Environment.GetEnvironmentVariable("HouseholdPlannerApiKey");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            


            var jwtIssuerOptions = new JwtIssuerOptions();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            jwtAppSettingOptions.Bind(jwtIssuerOptions);
            jwtIssuerOptions.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            var emailOptions = new EmailOptions();
            var emailAppSettingsOptions = Configuration.GetSection(nameof(EmailOptions));
            emailAppSettingsOptions.Bind(emailOptions);
            emailOptions.SendGridApiKey = sendGridApiKey;

            services.AddSingleton<JwtIssuerOptions>(jwtIssuerOptions);
            services.AddSingleton<EmailOptions>(emailOptions);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtIssuerOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtIssuerOptions.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(JwtConstants.JwtClaimIdentifiers.Rol, JwtConstants.JwtClaims.ApiAccess));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
