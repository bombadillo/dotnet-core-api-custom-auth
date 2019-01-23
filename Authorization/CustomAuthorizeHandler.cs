using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CustomAuthAttribute.Authorization
{
  public class CustomAuthorizeHandler : AuthorizationHandler<CustomAuthorize>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthorizeHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      var sessionId = GetSessionId();

      if (string.IsNullOrWhiteSpace(sessionId))
      {
        return UnAuthorized(context, requirement);
      }

      return Authorized(context, requirement);
    }

    private string GetSessionId()
    {
      var queries = _httpContextAccessor.HttpContext.Request.Query;
      var sessionIds = queries["session"];

      if (sessionIds.Count != 1)
      {
        return null;
      }

      return sessionIds[0];
    }

    private Task Authorized(AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      context.Succeed(requirement);

      return Task.CompletedTask;
    }

    private Task UnAuthorized(AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      context.Fail();

      return Task.CompletedTask;
    }
  }
}
