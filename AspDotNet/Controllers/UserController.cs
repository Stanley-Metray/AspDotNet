using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspDotNet.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspDotNet.Controllers
{
    
    public class UserController : Controller
    {
        private readonly UserService _user;

        public UserController(UserService user)
        {
            _user = user;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _user.GetUsersAsync();
            return View(users);
        }
    }
}

