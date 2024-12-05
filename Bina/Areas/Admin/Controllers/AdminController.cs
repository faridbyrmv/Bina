using Bina.Core.Dto.User;
using Bina.Core.Services.Interfaces;
using Bina.DataProvider.Response;
using Microsoft.AspNetCore.Mvc;

namespace Bina.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminController : Controller
{
    private readonly IUserService _service;
    public AdminController(IUserService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Register()
    {
         return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        if(result.Success)
            return View();
        return BadRequest(result);
    }

    public async Task<IActionResult> LogIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(RegistrationDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        if (result.Success)
            return View();
        return BadRequest(result);
    }
}
