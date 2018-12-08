using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HouseholdPlanner.Contracts.Common;
using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Contracts.Security;
using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using HouseholdPlanner.Data.EntityFramework.Repositories;
using HouseholdPlanner.Models.Options;
using HouseholdPlanner.Services;
using HouseholdPlanner.Services.Common;
using HouseholdPlanner.Services.Notification;
using HouseholdPlannerApi.Services.Account;
using HouseholdPlannerApi.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var sqlPassword = Environment.GetEnvironmentVariable("SqlServerPassword");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var jwtIssuerOptions = new JwtIssuerOptions();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            jwtAppSettingOptions.Bind(jwtIssuerOptions);
            jwtIssuerOptions.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            var emailOptions = new EmailOptions();
            Configuration.GetSection(nameof(EmailOptions)).Bind(emailOptions);

            var sendGridOptions = new SendGridOptions();
            Configuration.GetSection(nameof(SendGridOptions)).Bind(sendGridOptions);
            sendGridOptions.SendGridApiKey = sendGridApiKey;

			var applicationSettings = new ApplicationSettings();
			Configuration.GetSection(nameof(ApplicationSettings)).Bind(applicationSettings);

            services.AddSingleton<JwtIssuerOptions>(jwtIssuerOptions);
            services.AddSingleton<EmailOptions>(emailOptions);
            services.AddSingleton<SendGridOptions>(sendGridOptions);
			services.AddSingleton<ApplicationSettings>(applicationSettings);

			var httpClient = new HttpClient();
			services.AddSingleton<HttpClient>(httpClient);

			services.AddTransient<IRepositoryFactory, RepositoryFactory>();
			services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
			services.AddTransient<IFamilyService, FamilyService>();
			services.AddTransient<IMemberService, MemberService>();
            services.AddTransient<IInvitationService, InvitationService>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<IEmailService, SendGridEmailService>();
			services.AddTransient<ITokenFactory, TokenFactory>();
			services.AddTransient<IUserService, UserService>();

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

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            connectionString = string.Format(connectionString, sqlPassword);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("HouseholdPlanner.Data.EntityFramework")));

			var contextFactory = new DbContextFactory(connectionString);
			services.AddSingleton<IDbContextFactory<ApplicationDbContext>>(contextFactory);
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
