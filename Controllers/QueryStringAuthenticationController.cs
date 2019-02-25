using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CustomAuthAttribute.Controllers
{
  [Route ("api/[controller]")]
  [ApiController]
  public class QueryStringAuthenticationController : ControllerBase
  {
    [HttpGet ("login/{id}")]
    public IActionResult Login (int id)
    {
      IEnumerable<Claim> claims;

      switch (id)
      {
        case 1:
          claims = new List<Claim>
          {
            new Claim ("ValueById", ""),
            new Claim ("AllValues", ""),
          };
          break;

        default:
          claims = new List<Claim>
          {
            new Claim ("ValueById", ""),
          };
          break;

      }                 

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes("SECRET KEY IS THE BEST KEY");
      var tokenDescriptor = new SecurityTokenDescriptor
      {
          Subject = new ClaimsIdentity(claims),
          Expires = DateTime.UtcNow.AddMinutes(10),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };            
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var tokenString = tokenHandler.WriteToken(token);      

      return Ok (tokenString);
    }
  }
}