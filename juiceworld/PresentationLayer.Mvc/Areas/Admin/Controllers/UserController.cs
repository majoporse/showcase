using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Admin.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Controllers;

[Area(Constants.Areas.Admin)]
[RedirectIfNotAdminActionFilter]
public class UserController(IUserService userService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UserFilterViewModel userFilterViewModel)
    {
        var users = await userService.GetUsersFilteredAsync(mapper.Map<UserFilterDto>(userFilterViewModel));
        return View(new FilteredResult<UserSimpleViewModel>
        {
            Entities = mapper.Map<IEnumerable<UserDto>, IEnumerable<UserSimpleViewModel>>(users.Entities),
            PageIndex = users.PageIndex,
            TotalPages = users.TotalPages
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new UserRegisterViewModel
        {
            UserName = "",
            Email = "",
            Password = "",
            ConfirmPassword = "",
            Bio = ""
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserRegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var createdUser = await userService.RegisterUserAsync(
            mapper.Map<UserRegisterDto, UserRegisterDto>(mapper.Map<UserRegisterDto>(viewModel)), viewModel.UserRole);

        if (!createdUser.Succeeded)
        {
            ModelState.AddModelError("Email", "A user with this username or email already exists.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
        {
            ModelState.AddModelError("Id", "User not found.");
            return View();
        }

        return View(mapper.Map<UserUpdateViewModel>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserUpdateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        if (string.IsNullOrEmpty(viewModel.Password))
            viewModel.Password = null; // Do not update password if it is empty

        var updatedUser = await userService.UpdateUserAsync(mapper.Map<UserUpdateDto>(viewModel));
        if (updatedUser == null)
        {
            ModelState.AddModelError("Email", "A user with this username or email already exists.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await userService.DeleteUserByIdAsync(id);
        if (!deleted)
        {
            return View(Constants.Views.BadRequest);
        }

        return RedirectToAction(nameof(Index));
    }
}
