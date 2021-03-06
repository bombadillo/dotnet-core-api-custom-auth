﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomAuthAttribute.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomAuthAttribute
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
      services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

      services.AddAuthorization (options =>
      {
        options.AddPolicy ("AllValues", policy => policy.Requirements.Add (new CustomAuthorize ("AllValues")));
        options.AddPolicy ("ValueById", policy => policy.Requirements.Add (new CustomAuthorize ("ValueById")));
      });

      services.AddSingleton<IAuthorizationHandler, CustomAuthorizeHandler> ();

      services.AddAuthentication (CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie (options =>
        {
          options.Events.OnRedirectToLogin = (context) =>
          {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
          };

          options.Events.OnRedirectToAccessDenied = (context) =>
          {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
          };
        });

      services.AddHttpContextAccessor ();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment ())
      {
        app.UseDeveloperExceptionPage ();
      }

      app.UseAuthentication ();

      app.UseMvc ();
    }
  }
}