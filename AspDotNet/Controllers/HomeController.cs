using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspDotNet.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;

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
            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.UserId = userId;
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
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionDetails != null)
        {
            var exception = exceptionDetails.Error;
            _logger.LogError(exception, "An error occurred.");
            ViewBag.ErrorMessage = exception.Message;  // Send the error message to the view
        }
        return View();
    }

    public IActionResult ThrowError()
    {
        throw new Exception("This is a test exception to check global error handling!");
    }

}

