using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoStorage.Core;
using PhotoStorage.Service;

namespace PhotoStorage.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly IAzureFileStorageService _azureFileStorageService;
        public PhotoController(IAzureFileStorageService azureFileStorageService)
        {
            _azureFileStorageService = azureFileStorageService;
        }

        [HttpPost]
        public async void Post(IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0 && ImageProcessor.Formats.Contains(Path.GetExtension(photo.FileName).ToLowerInvariant()))
                {
                    var fileName = Path.GetFileName(photo.FileName);

                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        await _azureFileStorageService.SaveFileToAzureStorage("test", fileName, memoryStream);
                    }
                }
            }
        }
    }
}
