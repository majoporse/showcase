using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
public class UserController(IUserService userService) : ControllerBase
{
    private const string ApiBaseName = "User";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(RegisterUser))]
    public async Task<ActionResult<UserDto>> RegisterUser(UserRegisterDto user)
    {
        var result = await userService.RegisterUserAsync(user);
        return result.Succeeded ? Ok(result) : Problem();
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllUsers))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var result = await userService.GetAllUsersAsync();
        return Ok(result);
    }

    [HttpGet("{userId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetUser))]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userIdParsed)) return Unauthorized();
        if (!User.IsInRole(UserRole.Admin.ToString()) &&
            userIdParsed != userId)
            return Unauthorized();
        var result = await userService.GetUserByIdAsync(userId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateUser))]
    public async Task<ActionResult<UserDto>> UpdateUser(UserUpdateDto user)
    {
        var result = await userService.UpdateUserAsync(new UserUpdateDto
        {
            Id = user.Id,
            Bio = user.Bio,
            Password = user.Password,
            ConfirmPassword = user.ConfirmPassword,
            UserRole = user.UserRole
        });

        if (result == null)
            return NotFound();

        if (User.IsInRole(UserRole.Admin.ToString())) return Ok(result);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();
        return user.Id != userId ? NotFound() : Ok(result);
    }

    [HttpDelete("{userId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteUser))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var result = await userService.DeleteUserByIdAsync(userId);
        return result ? Ok() : NotFound();
    }
}