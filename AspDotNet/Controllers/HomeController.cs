using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspDotNet.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AspDotNet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.Claims;
            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.GoogleId = googleId;
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

