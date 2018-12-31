using FileService.Api.Filters;
using FileService.Api.Models;
using FileService.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Api
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(GetJwtBearerOptions);
            services.AddResponseCaching();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .Build());
            });
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ValidateModelStateAttribute());
                    options.Filters.Add(new MyHandleErrorAttribute());
                    options.CacheProfiles.Add("default", new CacheProfile { Duration = 60 * 20, });
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseResponseCaching();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");
            });
        }
        public void GetJwtBearerOptions(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["jwt:issuer"],
                ValidAudience = Configuration["jwt:issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"]))
            };
            options.Events = new JwtBearerEvents();
            options.Events.OnChallenge = context =>
            {
                if (context.Error == "invalid_token")
                {
                    context.HandleResponse();
                    var response = new ResponseItem<string>(ErrorCode.unauthorized, context.ErrorDescription);
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(JsonSerializerHelper.Serialize(response));
                }
                return Task.CompletedTask;
            };
            options.Events.OnMessageReceived = context =>
            {
                string query_token = context.Request.Query["access_token"];
                string cookies_token = context.Request.Cookies["access_token"];
                string header_token = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(header_token))
                {
                    context.Token = header_token;
                }
                else if (!string.IsNullOrEmpty(query_token))
                {
                    context.Token = query_token;
                }
                else if (!string.IsNullOrEmpty(cookies_token))
                {
                    context.Token = cookies_token;
                }
                if (context.Token == null) context.Token = "none";
                return Task.CompletedTask;
            };
        }
    }
}
