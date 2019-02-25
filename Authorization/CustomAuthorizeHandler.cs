using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
      var queryStringToken = GetQueryStringToken ();
      var userLoggedIn = context.User.Identity.IsAuthenticated;

      if (queryStringToken != null)
      {
        return HandleSourceSystem (context, requirement, queryStringToken);
      }

      if (userLoggedIn)
      {
        return HandleUser (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }

    private JwtSecurityToken GetQueryStringToken () 
    {
      var queries = _httpContextAccessor.HttpContext.Request.Query;
      var sessionIds = queries["session"];

      if (sessionIds.Count != 1)
      {
        return null;
      }

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.ReadToken(sessionIds[0]) as JwtSecurityToken;

      if (ValidateQueryStringToken(token))
      {
        return token;
      }

      return null;
    }    

    private bool ValidateQueryStringToken (JwtSecurityToken token)
    {
      return token != null;
    }    

    private Task HandleSourceSystem (AuthorizationHandlerContext context, CustomAuthorize requirement, JwtSecurityToken token)
    {
      if (UserHasClaims (token.Claims, requirement))
      {
        return Authorized (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }    

    private bool UserHasClaims (IEnumerable<Claim> claims, CustomAuthorize requirement)
    {
      foreach (var claim in claims)
      {
        if (claim.Type == requirement.Role)
        {
          return true;
        }
      }

      return false;
    }    

    private Task HandleUser (AuthorizationHandlerContext context, CustomAuthorize requirement)
    {
      if (UserHasClaims (context.User.Claims, requirement))
      {
        return Authorized (context, requirement);
      }

      return UnAuthorized (context, requirement);
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