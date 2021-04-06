using MechanicChecker.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /*
         * Allows variable to be accessed from any class.
         * Needed to get the secret keys from appsettings.json based on class context
         */
        internal static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            //    Configuration["Data:MechanicChecker:ConnectionString"]));

            services.Add(new ServiceDescriptor(typeof(LocalProductContext), new LocalProductContext(Configuration.GetConnectionString("DefaultConnectionString"))));
            services.Add(new ServiceDescriptor(typeof(SellerProductContext), new SellerProductContext(Configuration.GetConnectionString("DefaultConnectionString"))));
            services.Add(new ServiceDescriptor(typeof(SellerAddressContext), new SellerAddressContext(Configuration.GetConnectionString("DefaultConnectionString"))));

            //services.AddTransient<ILocalProductRepository, EFLocalProductRepository>();
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "",
                    template: "{controller=Search}/{action=Search}");
            });
        }
    }
}
