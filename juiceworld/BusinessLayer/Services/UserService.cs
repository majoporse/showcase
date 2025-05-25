using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLayer.Services;

public class UserService(
    IRepository<User> userRepository,
    IQueryObject<User> userQueryObject,
    UserManager<User> userManager,
    IMapper mapper) : IUserService
{
    public async Task<IdentityResult> RegisterUserAsync(UserRegisterDto userRegisterDto, UserRole role)
    {
        if (await GetUserByEmailAsync(userRegisterDto.Email) is not null) return IdentityResult.Failed();

        var user = new User
        {
            UserName = userRegisterDto.UserName,
            Email = userRegisterDto.Email,
            Bio = userRegisterDto.Bio,
            UserRole = role
        };

        var result = await userManager.CreateAsync(user, userRegisterDto.Password);
        return result;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }

    public Task<FilteredResult<UserDto>> GetUsersFilteredAsync(UserFilterDto userFilter)
    {
        var query = userQueryObject
            .Filter(user => user.UserName != null && user.Email != null && (userFilter.Name == null ||
                                                                            user.UserName.ToLower()
                                                                                .Contains(userFilter.Name.ToLower())
                                                                            || user.Email.ToLower()
                                                                                .Contains(userFilter.Name.ToLower())))
            .Paginate(userFilter.PageIndex, userFilter.PageSize)
            .OrderBy(user => user.Id);

        return query.ExecuteAsync().ContinueWith(result => new FilteredResult<UserDto>
        {
            Entities = mapper.Map<List<UserDto>>(result.Result.Entities),
            PageIndex = result.Result.PageIndex,
            TotalPages = result.Result.TotalPages
        });
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = (await userQueryObject.Filter(user => user.Email == email).ExecuteAsync()).Entities.FirstOrDefault();
        return user is null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user is null ? null : mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateUserAsync(UserUpdateDto userDto)
    {
        var user = await userManager.FindByIdAsync(userDto.Id.ToString());
        if (user == null) return null;

        if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;

        // Update only non-null fields
        if (!string.IsNullOrEmpty(userDto.UserName)) user.UserName = userDto.UserName;

        if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;

        if (!string.IsNullOrEmpty(userDto.Bio)) user.Bio = userDto.Bio;
        user.UserRole = userDto.UserRole;

        user.UpdatedAt = DateTime.UtcNow;

        // Save changes
        await userManager.UpdateAsync(user);

        if (userDto.Password != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, userDto.Password);
        }

        var userDb = await userRepository.GetByIdAsync(userDto.Id);

        return mapper.Map<UserDto>(userDb);
    }

    public Task<bool> DeleteUserByIdAsync(int id)
    {
        return userRepository.DeleteAsync(id);
    }
}