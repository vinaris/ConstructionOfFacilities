using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using static System.String;
using static ConstructionOfFacilities.Views.ConvertFunctions;

namespace ConstructionOfFacilities.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class ImagesController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;

        public ImagesController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public ActionResult Index()
        {
            var images = new Dictionary<string, List<string>>();
            var path = "wwwroot/images/objects/";
            var directories = new List<string>(Directory.EnumerateDirectories(path));
            foreach (var directory in directories)
            {
                var dirName = directory.Split('/').Last();
                var tempFilesList = new List<string>(Directory.GetFiles(directory));
                var filesList = new List<string>();
                foreach (var file in tempFilesList)
                {
                    filesList.Add(file.Replace("wwwroot/", ""));
                }
                images.Add(dirName, filesList);
            }
            ViewBag.Dictionary = images;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddImageToObject(string directory, List<IFormFile> files)
        {
            foreach (var formFile in files)
            {
                var fullPath = "/images/objects/" + directory + "/" + formFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + fullPath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewAddress(string newDirectory, List<IFormFile> files)
        {
            var path = "wwwroot/images/objects/";
            var directories = new List<string>(Directory.EnumerateDirectories(path));
            var dirName = newDirectory;
            var flag = false;
            foreach (var directory in directories)
            {
                var str = directory.Split('/').Last();
                if (ClearAddressString(newDirectory) != ClearAddressString(str)) continue;
                flag = true;
                dirName = str;
                break;
            }
            var folderPath = "/images/objects/" + dirName;
            if (!flag)
            {
                Directory.CreateDirectory(_appEnvironment.WebRootPath + folderPath);
                
            }
            foreach (var formFile in files)
            {
                var fullPath = "/images/objects/" + dirName + "/" + formFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + fullPath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveImageFromObject(string src)
        {
            var fullPath = _appEnvironment.WebRootPath + "/" + src;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteObject(string folderName)
        {
            if (IsNullOrEmpty(folderName)) return RedirectToAction("Index");
            var fullPath = _appEnvironment.WebRootPath + "/images/objects/" + folderName;
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
            return RedirectToAction("Index");
        }
    }
}