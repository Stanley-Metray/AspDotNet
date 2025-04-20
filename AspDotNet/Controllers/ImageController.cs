using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspDotNet.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspDotNet.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }
        
        public IActionResult CacheImageUrl()
        {
            string imageUrl = "https://picsum.photos/400/300?random=1";
            string cachedUrl = _imageService.GetCachedImageUrl(imageUrl);

            ViewBag.ImageUrl = cachedUrl;
            return View();
        }

        public async Task<IActionResult> CacheImage()
        {
            string imageUrl = "https://picsum.photos/400/300?random=2";
            string localPath = await _imageService.GetCachedImagePathAsync(imageUrl);

            string fileName = Path.GetFileName(localPath);
            ViewBag.ImagePath = $"/cachedImages/{fileName}";
            return View();
        }
    }
}

