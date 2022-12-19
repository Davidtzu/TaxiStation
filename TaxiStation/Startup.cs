using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TaxiStation.Helper;

namespace Template_MVC_Vue
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();
            services.AddSpaStaticFiles(opts => opts.RootPath = "wwwroot");
            services.AddControllersWithViews().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; }); ;
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(this.Configuration);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            var accesor = svp.GetService<IHttpContextAccessor>();
            var Configuration = this.Configuration;
            Helpers.SetInitElements(accesor, Configuration);

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

            app.UseRouting();

            //app.UseCors(IISOptions:CorsPolicyBuilder => options
            //    .WithOrigins(
            //    new string[]{
            //        "https://localhost:3000", "https://localhost:8080",
            //        "https://localhost:4200", "https://localhost:5000"
            //    }).AllowAnyHeader().AllowAnyMethod()
            //    );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Main}/{action=Index}/{id?}");
            });

            app.UseSpaStaticFiles();
            app.UseSpa(builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });
        }
    }
}
