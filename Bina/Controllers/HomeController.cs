using System.Diagnostics;
using Bina.Core.Dto.Homes;
using Bina.Core.Services.Interfaces;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bina.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _service;

        public HomeController(IHomeService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var res = await _service.GetAllAsync();
            if (res.Success)
                return View(res);
            return View(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(long id)
        {
            var res = await _service.GetByIdAsync(id);
            if (res.Success)
                return View(res);
            return View(res);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHomeDto dto)
        {
            var res = await _service.CreateAsync(dto);
            if(res.Success)
                return RedirectToAction("Index");
            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var res = await _service.DeleteAsync(id);
            if (res.Success)
                return RedirectToAction("Index");
            return View(res);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
