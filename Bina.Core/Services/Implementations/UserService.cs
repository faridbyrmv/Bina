using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using Bina.Core.Dto.User;
using Bina.DataProvider.Response;
using Bina.Mail;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Bina.Core.Services.Interfaces;
using Bina.Core.Helpers;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;

    public UserService(UserManager<IdentityUser> userManager,
                       SignInManager<IdentityUser> signInManager,
                       RoleManager<IdentityRole> roleManager,
                       IMailService mailService,
                       IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _mailService = mailService;
        _configuration = configuration;
    }

    public async Task<BaseResponse<IdentityUser>> RegisterAsync(RegistrationDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                Log.Warning("User registration failed. Email {Email} is already in use.", dto.Email);
                return new BaseResponse<IdentityUser>(null, false, "User with this email already exists.");
            }

            var newUser = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                Log.Error("User registration failed for email {Email}. Errors: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new BaseResponse<IdentityUser>(null, false, "Failed to register user.");
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var domain = _configuration["App:Domain"] ?? "localhost:7152";
            var encodedToken = Uri.EscapeDataString(token);
            var link = $"https://{domain}/User/Verifyemail?token={encodedToken}&email={newUser.Email}";
            await _mailService.Send("hacibalaev.azik@mail.ru", newUser.Email, link, "Verify Email");

            Log.Information("User registered successfully. Verification email sent to {Email}.", newUser.Email);
            return new BaseResponse<IdentityUser>(newUser, true, "User registered successfully. Verification email sent.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during user registration for email {Email}.", dto.Email);
            return new BaseResponse<IdentityUser>(null, false, "An error occurred while registering the user.");
        }
    }

    public async Task<BaseResponse<IdentityUser>> VerifyEmailAsync(string token, string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Log.Warning("Email verification failed. User with email {Email} not found.", email);
                return new BaseResponse<IdentityUser>(null, false, "User not found.");
            }

            Log.Information("Verifying email for user {Email} with token {Token}.", email, token);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Log.Information("Email successfully verified for user {Email}.", email);
                return new BaseResponse<IdentityUser>(user, true, "Email verified successfully.");
            }
            else
            {
                Log.Error("Email verification failed for user {Email}. Errors: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new BaseResponse<IdentityUser>(null, false, "Email verification failed.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while verifying email for user {Email}.", email);
            return new BaseResponse<IdentityUser>(null, false, "An error occurred while verifying email.");
        }
    }

    public async Task<BaseResponse<string>> LoginAsync(LogInDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.UserName);
            if (user == null)
            {
                Log.Warning("Login failed. User {UserName} not found.", dto.UserName);
                return new BaseResponse<string>(null, false, "User not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
            {
                Log.Warning("Login failed for user {UserName}. Invalid credentials.", dto.UserName);
                return new BaseResponse<string>(null, false, "Invalid login credentials.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtHelper = new GenerateJwtHelper();
            var tokenString = jwtHelper.GenerateJwtToken(authClaims);

            Log.Information("Login successful for user {UserName}.", dto.UserName);
            return new BaseResponse<string>(tokenString, true, "Login successful.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during login for user {UserName}.", dto.UserName);
            return new BaseResponse<string>(null, false, "An error occurred while logging in.");
        }
    }

    public async Task<BaseResponse<IdentityUser>> LogoutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            Log.Information("User logged out successfully.");
            return new BaseResponse<IdentityUser>(null, false, "Logged out successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during logout.");
            return new BaseResponse<IdentityUser>(null, false, "An error occurred while logging out.");
        }
    }

    public async Task<BaseResponse<bool>> RemoveUserAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Log.Warning("Remove user failed. User with ID {UserId} not found.", userId);
                return new BaseResponse<bool>(false, false, "User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                Log.Information("User with ID {UserId} removed successfully.", userId);
                return new BaseResponse<bool>(true, true, "User removed successfully.");
            }

            Log.Error("Failed to remove user with ID {UserId}.", userId);
            return new BaseResponse<bool>(false, false, "Failed to remove user.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while removing user with ID {UserId}.", userId);
            return new BaseResponse<bool>(false, false, "An error occurred while removing user.");
        }
    }

    public async Task<BaseResponse<IEnumerable<GetUserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = _userManager.Users
                .Select(user => new GetUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                })
                .ToList();

            Log.Information("Retrieved {Count} users.", users.Count);
            return new BaseResponse<IEnumerable<GetUserDto>>(users, true, "Users retrieved successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while retrieving users.");
            return new BaseResponse<IEnumerable<GetUserDto>>(null, false, "Failed to retrieve users.");
        }
    }
}
