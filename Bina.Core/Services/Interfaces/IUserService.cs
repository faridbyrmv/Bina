using Bina.Core.Dto.User;
using Bina.DataProvider.Response;
using Microsoft.AspNetCore.Identity;

namespace Bina.Core.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponse<IdentityUser>> RegisterAsync(RegistrationDto dto);
    Task<BaseResponse<IdentityUser>> VerifyEmailAsync(string token, string email);
    Task<BaseResponse<string>> LoginAsync(LogInDto dto);
    Task<BaseResponse<IdentityUser>> LogoutAsync();
    Task<BaseResponse<IEnumerable<GetUserDto>>> GetAllUsersAsync();
    Task<BaseResponse<bool>> RemoveUserAsync(string userId);
}

