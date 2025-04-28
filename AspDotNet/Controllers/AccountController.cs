using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult SignInWithGoogle()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/Home/Index"  // Redirect to the homepage after login
        }, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult SignInWithMicrosoft()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/Home/Index"  // Redirect to the homepage after login
        }, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
