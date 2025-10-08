using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspDotNet.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspDotNet.Controllers
{
    public class TestController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewData["FirstName"] = "Stanley";
            ViewBag.FirstName = "Stephen";
            return View();
        }
    }
}

