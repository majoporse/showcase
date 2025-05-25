using System.Security.Claims;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Customer.Models;
using PresentationLayer.Mvc.Models;
using Index = System.Index;

namespace PresentationLayer.Mvc.Areas.Customer.Controllers;

[Area(Constants.Areas.Customer)]
public class UserController(
    IUserService userService,
    IMapper mapper,
    SignInManager<User> signInManager) : Controller
{
    // GET: /User/Register
    [HttpGet]
    public ActionResult Register()
    {
        return View();
    }

    // POST: /User/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(UserRegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await userService.RegisterUserAsync(new UserRegisterDto
        {
            UserName = model.UserName,
            Email = model.Email,
            Bio = model.Bio,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        });

        var userDb = await signInManager.UserManager.FindByEmailAsync(model.Email);
        if (userDb is null)
        {
            ModelState.AddModelError(string.Empty, "User doesn't exist.");
            return Unauthorized();
        }

        if (result.Succeeded)
        {
            var ci = new[]
            {
                new Claim(ClaimTypes.Sid, userDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userDb.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, userDb.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, userDb.UserRole.ToString())
            };

            await signInManager.SignInWithClaimsAsync(userDb, false, ci);
            return RedirectToAction(nameof(Index), "Home");
        }

        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

        return RedirectToAction("Index", "Home");
    }

    //GET: /User/Login
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    // POST: /User/Login
    [HttpPost]
    public async Task<ActionResult> Login(UserLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Add error message by default, redirect on success
        ModelState.AddModelError(nameof(UserLoginViewModel.Email), "Invalid username or password.");

        var user = await signInManager.UserManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return View(model);
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (result.Succeeded)
        {
            var ci = new[]
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            await signInManager.SignInWithClaimsAsync(user, false, ci);
            return RedirectToAction(nameof(Index), "Home");
        }

        return View(model);
    }

    // GET: /User/Logout
    [HttpGet]
    public ActionResult Logout()
    {
        signInManager.SignOutAsync();
        return RedirectToAction("Login", "User");
    }

    // GET: /User/Profile
    [HttpGet]
    [RedirectIfNotAuthenticatedActionFilter]
    public async Task<ActionResult> Profile()
    {
        var user = await userService.GetUserByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value ?? "");
        if (user is null)
        {
            return View(Constants.Views.NotFound);
        }

        return View(mapper.Map<UserProfileViewModel>(user));
    }

    [HttpGet]
    [RedirectIfNotAuthenticatedActionFilter]
    public async Task<IActionResult> Edit()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.Sid)?.Value, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null) return View(Constants.Views.BadRequest);

        return View(mapper.Map<UserUpdateRestrictedViewModel>(user));
    }

    [HttpPost]
    [RedirectIfNotAuthenticatedActionFilter]
    public async Task<IActionResult> Edit(UserUpdateRestrictedViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.Sid)?.Value, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        if (viewModel.Id != userId)
        {
            return View(Constants.Views.BadRequest);
        }

        if (string.IsNullOrEmpty(viewModel.Password))
            viewModel.Password = null; // Do not update password if it is empty

        var updatedUser =
            await userService.UpdateUserAsync(mapper.Map<UserUpdateRestrictedViewModel, UserUpdateDto>(viewModel));
        if (updatedUser == null)
        {
            ModelState.AddModelError("Email", "A user with this username or email already exists.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Profile));
    }
}
