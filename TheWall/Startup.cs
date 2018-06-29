using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheWall.Models;

namespace TheWall
{
  public class Startup
  {
    public Startup (IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices (IServiceCollection services)
    {
      services.AddMvc ();
      services.AddSession ();
      services.AddDbContext<DataContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));
      services.AddIdentity<User, IdentityRole> ()
        .AddEntityFrameworkStores<DataContext> ()
        .AddDefaultTokenProviders ();
      services.Configure<IdentityOptions> (options =>
      {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 6;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
      });

      services.ConfigureApplicationCookie (options =>
      {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes (30);
        // If the LoginPath isn't set, ASP.NET Core defaults 
        // the path to /Account/Login.
        options.LoginPath = "/Account/Login";
        // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
        // the path to /Account/AccessDenied.
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment ())
      {
        app.UseDeveloperExceptionPage ();
      }
      else
      {
        app.UseExceptionHandler ("/Home/Error");
      }

      app.UseStaticFiles ();
      app.UseSession ();
      app.UseMvc (routes =>
      {
        routes.MapRoute (
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}