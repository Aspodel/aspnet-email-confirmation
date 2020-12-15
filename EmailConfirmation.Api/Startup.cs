using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailConfirmation.Api.Database;
using EmailConfirmation.Api.Models;
using EmailConfirmation.Api.Services;
using EmailConfirmation.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailConfirmation.Api
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
            services.AddControllers();


            services.Configure<JwTokenConfig>(Configuration.GetSection("JwtKey"));


            services.AddDbContext<EmailConfirmationContext>(options =>
                                               options.UseSqlServer
                                               (Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                //options.Password.RequiredLength = 6;

                //options.User.RequireUniqueEmail = true;
                //options.SignIn.RequireConfirmedEmail = true;
            })
                    .AddEntityFrameworkStores<EmailConfirmationContext>()
                    .AddDefaultTokenProviders();


            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<EmailConfig>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
