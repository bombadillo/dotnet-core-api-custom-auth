using System;
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
        return Authorized (context, requirement);
      }

      if (userLoggedIn)
      {
        return Authorized (context, requirement);
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