using System.Security.Claims;
using BusinessLayer.DTOs;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    SignInManager<User> signInManager) : ControllerBase
{
    private const string ApiBaseName = "CartItem";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(Login))]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginDto login)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User doesn't exist.");
            return Unauthorized();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
        if (!result.Succeeded) return Unauthorized();

        var ci = new[]
        {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        await signInManager.SignInWithClaimsAsync(user, false, ci);
        return Ok();
    }
}