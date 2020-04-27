using Cnf.Finance.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cnf.Finance.Web
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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<IApiConnector, ApiConnector>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IPerformService, PerformService>();
            services.AddScoped<IAnalysisService, AnalysisService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Home/Auth";
                        options.Cookie.HttpOnly = true;
                    });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
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
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value.ToLower();
                if (path == "/" || path.StartsWith("/home"))
                {
                    await next.Invoke();
                }
                else if (path.StartsWith("/system"))
                {
                    if (Helper.IsSystemAdmin(context))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Denied");
                    }
                }
                else if (path.StartsWith("/project"))
                {
                    //if (path.StartsWith("/project/getqualifystate")
                    //      || path.StartsWith("/project/getdutyqualifications")
                    //      || path.StartsWith("/project/getcertsofemployee")
                    //      || path.StartsWith("/project/verifytransfer")
                    //      || path.StartsWith("/project/searchprojects"))
                    //{
                    //    //ajax api, won't authenticate them
                    //    await next.Invoke();
                    //}
                    //else
                    //{
                    if (Helper.IsPlanner(context) || Helper.IsReporter(context) || Helper.IsSupervisor(context))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Denied");
                    }
                    //}
                }
                else if (path.StartsWith("/plan"))
                {
                    if (Helper.IsPlanner(context) || Helper.IsSupervisor(context))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Denied");
                    }
                }
                else if (path.StartsWith("/perform"))
                {
                    if (Helper.IsReporter(context) || Helper.IsSupervisor(context))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Denied");
                    }
                }
                else if (path.StartsWith("/analysis"))
                {
                    if (Helper.IsSupervisor(context))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Denied");
                    }
                }
                else
                {
                    context.Response.Redirect("/Home/Denied");
                }

            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
