using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthAttribute.Controllers
{
  [Route ("api/[controller]")]
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
    [HttpGet ("login")]
    public async Task<IActionResult> Login (CancellationToken cancellationToken)
    {
      var claimsIdentity = new ClaimsIdentity (
        null, CookieAuthenticationDefaults.AuthenticationScheme);

      var authProperties = new AuthenticationProperties
      {
        AllowRefresh = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes (10),
        IsPersistent = true,
        IssuedUtc = DateTime.UtcNow
      };

      await HttpContext.SignInAsync (
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal (claimsIdentity),
        authProperties);

      return Ok ();
    }

    [HttpGet ("logout")]
    public async Task<IActionResult> Logout ()
    {
      await HttpContext.SignOutAsync (
        CookieAuthenticationDefaults.AuthenticationScheme);

      return Ok ();
    }
  }
}