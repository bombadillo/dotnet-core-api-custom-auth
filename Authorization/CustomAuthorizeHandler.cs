using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CustomAuthAttribute.Authorization
{
  public class CustomAuthorizeHandler : AuthorizationHandler<CustomAuthorize>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthorizeHandler (IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      var validSessionId = ValidateSessionId ();
      var userLoggedIn = context.User.Identity.IsAuthenticated;

      if (validSessionId)
      {
        return HandleSourceSystem (context, requirement);
      }

      if (userLoggedIn && UserHasClaims (context, requirement))
      {
        return HandleUser (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }

    private bool ValidateSessionId ()
    {
      var queries = _httpContextAccessor.HttpContext.Request.Query;
      var sessionIds = queries["session"];

      if (sessionIds.Count != 1)
      {
        return false;
      }

      return !string.IsNullOrWhiteSpace (sessionIds[0]);
    }

    private Task HandleSourceSystem (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      if (SourceSystemHasClaims (context, requirement))
      {
        return Authorized (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }

    private Task HandleUser (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      if (UserHasClaims (context, requirement))
      {
        return Authorized (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }

    private bool SourceSystemHasClaims (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      var claims = new List<string> { "ValueById" };

      return claims.Contains (requirement.Role);
    }

    private bool UserHasClaims (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      var claims = context.User.Claims;

      foreach (var claim in claims)
      {
        if (claim.Type == requirement.Role)
        {
          return true;
        }
      }

      return false;
    }

    private Task Authorized (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      context.Succeed (requirement);

      return Task.CompletedTask;
    }

    private Task UnAuthorized (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      context.Fail ();

      return Task.CompletedTask;
    }
  }
}