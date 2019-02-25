using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

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
      var queryStringTokenValue = GetQueryStringTokenValue ();
      var userLoggedIn = context.User.Identity.IsAuthenticated;

      if (queryStringTokenValue != null)
      {
        return HandleSourceSystem (context, requirement, queryStringTokenValue);
      }

      if (userLoggedIn)
      {
        return HandleUser (context, requirement);
      }

      return UnAuthorized (context, requirement);
    }

    private string GetQueryStringTokenValue()
    {
      var queries = _httpContextAccessor.HttpContext.Request.Query;
      var sessionIds = queries["session"];

      if (sessionIds.Count != 1)
      {
        return null;
      }

      return sessionIds[0];   
    }

    private JwtSecurityToken GetQueryStringJwtToken (string queryStringTokenValue) 
    {         
      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.ReadToken(queryStringTokenValue) as JwtSecurityToken;

      if (token == null)
      {
        return null;
      }

      return token;
    }    

    private bool ValidateQueryStringToken (string queryStringTokenValue)
    {
      var validationParameters = new TokenValidationParameters()
      {
          ValidateLifetime = true,
          ValidateAudience = false,
          ValidateIssuer = false,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SECRET KEY IS THE BEST KEY"))
      };

      try 
      {
        SecurityToken validatedToken;
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(queryStringTokenValue, validationParameters, out validatedToken);
      }
      catch(Exception) {
        return false;
      }      
      
      return true;
    }    

    private Task HandleSourceSystem (AuthorizationHandlerContext context, CustomAuthorize requirement, string queryStringTokenValue)
    {
      if (!ValidateQueryStringToken(queryStringTokenValue))
      {
        return UnAuthorized(context, requirement);
      }

      var jwtToken = GetQueryStringJwtToken(queryStringTokenValue);

      if (jwtToken == null)
      {
        return UnAuthorized(context, requirement);
      }

      if (UserHasClaims (jwtToken.Claims, requirement))
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