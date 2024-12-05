using Bina.Core.Dto.User;
using Bina.Core.Services.Interfaces;
using Bina.DataProvider.Response;
using Bina.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class UserController : Controller
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        if (result.Success)
        {
            TempData["Message"] = "Please check your email for a verification link.";
            return RedirectToAction("Register");
        }
        return BadRequest(result);
    }

    public IActionResult LogIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(LogInDto dto)
    {
        var result = await _service.LoginAsync(dto);
        if (result.Success)
        {
            return RedirectToAction("Dashboard");
        }
        return BadRequest(result);
    }

    public async Task<IActionResult> LogOut()
    {
        var result = await _service.LogoutAsync();
        if (result.Success)
        {
            return RedirectToAction("LogIn");
        }
        return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string token, string email)
    {
        var response = await _service.VerifyEmailAsync(token, email);

        if (response.Success)
        {
            return RedirectToAction("Login", "User");
        }
        else
        {
            return RedirectToAction("Login", "User");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await _service.GetAllUsersAsync();
        if (response.Success)
        {
            return View(response.Data); 
        }

        return View("Error");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _service.RemoveUserAsync(id);
        if (response.Success)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
