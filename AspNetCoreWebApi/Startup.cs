using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate; // Added for winodws-based authentication using Kerstel

using Microsoft.AspNetCore.Server.IISIntegration; // Added for winodws-based authentication using IIS Express.

namespace AspNetCoreWebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private IConfiguration _configuration;
        public Startup(IConfiguration configurarion, IWebHostEnvironment env) {
            string configfile= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/appsettings.json";
            _configuration= new ConfigurationBuilder()
                                      .SetBasePath(Directory.GetCurrentDirectory())
                                      .AddJsonFile(configfile)
                                      .Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMvc(options => options.EnableEndpointRouting= false);
            // services.AddControllers();
            
            // To implement Windows-based Authentication using IIS Express.
            // services.AddAuthentication(IISDefaults.AuthenticationScheme);

            // To implement Windows-Based Authentication using Kerstel
            // services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            // Added authorization policy to verify user identity.
            // services.AddAuthorization(opts=> {
            //     opts.AddPolicy("ValidateUser", policy=> policy.RequireAssertion(ctx => {
            //         if(ctx.User.Identity.Name== "LAPTOP-N9FAJOE5\\omen") 
            //           return true;
            //         return false;
            //       }
            //    ));
            // });

            // Implement Basic Authentication
            services.AddAuthentication("BASIC-AUTH").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BASIC-AUTH", null);

              services.AddAuthorization(opts=> {
                opts.AddPolicy("RolePolicy", policy=> policy.RequireClaim(ClaimTypes.Role, AppVariables.AppRoles.Administrator.ToString()));
            });

            services.AddAuthorization(opts=> {
                opts.AddPolicy("ValidateUser", policy=> policy.RequireAssertion(ctx => {
                    if(ctx.User.Identity.Name== "itsmetanmoy4") 
                      return true;
                    return false;
                  }
               ));
            });

            services.AddSwaggerGen(options => {
                options.SwaggerDoc("myApi" //swagger document name. must refer the same while specifying the swagger endpoint.
                        , new Microsoft.OpenApi.Models.OpenApiInfo {
                    Title= "My Api",
                    Version="1.0",
                    Description= "ASP .NET Core Api Application"
                });
                options.AddSecurityDefinition("basic", new OpenApiSecurityScheme  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.Http,  
                    Scheme = "basic",  
                    In = ParameterLocation.Header,  
                    Description = "Basic Authorization header using the Bearer scheme."  
                });  
                options.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  new OpenApiSecurityScheme  {  Reference = new OpenApiReference  {  
                                    Type = ReferenceType.SecurityScheme,  
                                    Id = "basic"  
                            }  
                        },  new string[] {}  
                    }  
                });  
            });
            ILoggerFactory factory= LoggerFactory.Create(builder => builder.AddConsole());
            services.AddSingleton(factory.CreateLogger("log"));
            
            services.Configure<ApplicationSettings>(_configuration);
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseEndpoints(endpoint => {
            //     endpoint.MapControllers();
            // });
            app.UseMvc(route => {
                route.MapRoute(
                    name: "default",
                    template: "{controller=Name}/{action=Name}/{id?}"
                );
            });
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/myApi/swagger.json", "MyApi"));
            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapGet("/", async context =>
            //     {
            //         await context.Response.WriteAsync("Hello World!");
            //     });
            // });
        }
    }
}
