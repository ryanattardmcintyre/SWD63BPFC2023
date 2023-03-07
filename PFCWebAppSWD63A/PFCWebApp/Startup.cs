using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using PFCWebApp.DataAccess;

namespace PFCWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            string credential_path = @"C:\Users\attar\Source\Repos\SWD63BPFC2023\PFCWebAppSWD63A\PFCWebApp\swd63b2023-08d5b155ddab.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
           
            services
                    .AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    })
                    .AddCookie()
                    .AddGoogle(options =>
                    {
                        options.ClientId = Configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    });

            string projectId = Configuration["projectid"].ToString();

            services.AddScoped<FirestoreBooksRepository>(provider => new FirestoreBooksRepository(projectId));
            services.AddScoped<FirestoreReservationsRepository>(provider => 
            new FirestoreReservationsRepository(projectId, new FirestoreBooksRepository(projectId)));

            services.AddScoped<CacheMenusRepository>(
                provider => 
              //  new CacheMenusRepository("redis-14410.c1.us-east1-2.gce.cloud.redislabs.com:14410,password="));
               new CacheMenusRepository("127.0.0.1:6379"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
