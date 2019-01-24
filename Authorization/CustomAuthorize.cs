using Microsoft.AspNetCore.Authorization;

namespace CustomAuthAttribute.Authorization
{
  public class CustomAuthorize : IAuthorizationRequirement
  {
    public CustomAuthorize (string role)
    {
      Role = role;
    }

    public string Role { get; }
  }
}