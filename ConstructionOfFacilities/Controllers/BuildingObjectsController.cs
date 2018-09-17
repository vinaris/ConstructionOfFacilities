using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionOfFacilities.Data;
using ConstructionOfFacilities.Models;
using OfficeOpenXml;
using ConstructionOfFacilities.Views;

namespace ConstructionOfFacilities.Controllers
{
    [Authorize(Roles = "Администратор, Пользователь")]
    public class BuildingObjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public BuildingObjectsController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Index(int? tableId, string searchString, bool subject, bool address, bool responsible, bool executor, bool partner, bool comments)
        {
            if (tableId == null)
            {
                tableId = 1;
            }
            ViewBag.SelectedTable = tableId;
            if (string.IsNullOrEmpty(searchString))
            {
                return View(await _context.BuildingObject.Where(o => o.TypeOfTable == tableId).ToListAsync());
            }
            return View(await _context.BuildingObject.Where(o =>
                o.TypeOfTable == tableId && (subject && o.Name.Contains(searchString) ||
                                             address && o.Address.Contains(searchString) ||
                                             responsible && o.Responsible.Contains(searchString) ||
                                             executor && o.Executor.Contains(searchString) ||
                                             partner && o.Partner.Contains(searchString) ||
                                             comments && o.Comments.Contains(searchString))).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingObject = await _context.BuildingObject
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buildingObject == null)
            {
                return NotFound();
            }

            var path = "wwwroot/images/objects/";
            List<string> images = new List<string>();
            List<string> directories = new List<string>(Directory.EnumerateDirectories(path));

            var adresses = new List<string>();
            var mapAddress = "";
            if (!string.IsNullOrEmpty(buildingObject.Address))
            {
                if (buildingObject.Address.Contains(';'))
                {
                    adresses = buildingObject.Address.Split(';').ToList();
                    mapAddress = adresses[0]; // Отображение на карте первого адреса
                }                           
                else
                {
                    adresses.Add(buildingObject.Address);
                    mapAddress = buildingObject.Address;
                }
                foreach (var adr in adresses)
                {
                    foreach (var directory in directories)
                    {
                        var dirName = directory.Split('/').Last();
                        if (ConvertFunctions.ClearAddressString(dirName) == ConvertFunctions.ClearAddressString(adr))
                        {
                            var tempFilesList = new List<string>(Directory.GetFiles(directory));
                            foreach (var file in tempFilesList)
                            {
                                images.Add(file.Replace("wwwroot/", ""));
                            }
                        }
                    }
                }
            }
            ViewBag.Images = images;
            ViewBag.Address = mapAddress;
            return View(buildingObject);
        }

        public async Task<IActionResult> ObjectDetails(string id, int tableId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var filteredList = await _context.BuildingObject.Where(m => (m.Name == id || m.Responsible == id) && m.TypeOfTable == tableId).ToListAsync();
            if (filteredList == null)
            {
                return NotFound();
            }

            foreach (var buildingObject in filteredList)
            {
                if (buildingObject.Address == null) continue;
                var adr = buildingObject.Address.Contains(";") ? buildingObject.Address.Split(';')[0] : buildingObject.Address; // Временное решение, до реализации возможности множественного отображения объектов на карте
                ViewBag.Address = adr;
                break;
            }
            ViewBag.Filter = id;
            return View(filteredList);
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create(int tableId)
        {
            ViewBag.SelectedTable = Convert.ToInt32(tableId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Responsible,Amount,PriceOfContract,Executor,PublishDate,EndDate,BidsDate,AuctionDate,Bids,Partner,AmountOfContract,DateOfContract,Stage,StageOfContract,SourceOfFinancing, TypeOfTable")] BuildingObject buildingObject)
        {
            if (ModelState.IsValid)
            {
                buildingObject.LastUpDateTime = DateTime.Now;
                _context.Add(buildingObject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index",  buildingObject.TypeOfTable);
            }
            return View(buildingObject);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingObject = await _context.BuildingObject.SingleOrDefaultAsync(m => m.Id == id);
            if (buildingObject == null)
            {
                return NotFound();
            }
            return View(buildingObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Responsible,Amount,PriceOfContract,Executor,PublishDate,EndDate,BidsDate,AuctionDate,Bids,Partner,AmountOfContract,DateOfContract,Stage,StageOfContract,SourceOfFinancing,TypeOfTable")] BuildingObject buildingObject)
        {
            if (id != buildingObject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    buildingObject.LastUpDateTime = DateTime.Now;
                    _context.Update(buildingObject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingObjectExists(buildingObject.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(buildingObject);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingObject = await _context.BuildingObject
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buildingObject == null)
            {
                return NotFound();
            }

            return View(buildingObject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buildingObject = await _context.BuildingObject.SingleOrDefaultAsync(m => m.Id == id);
            _context.BuildingObject.Remove(buildingObject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BuildingObjectExists(int id)
        {
            return _context.BuildingObject.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Администратор")]
        public ActionResult UploadData(int? tableId)
        {
            ViewBag.SelectedTable = Convert.ToInt32(tableId);
            return View("UploadData");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public ActionResult DownloadExcel(int? tableId)
        {
            var files = Request.Form.Files;
            if (files == null) return RedirectToAction("Index");
            var stream = files[0].OpenReadStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                for (var i = workSheet.Dimension.Start.Row + 2; i <= workSheet.Dimension.End.Row; i++)
                {
                    if ((tableId != 1 && tableId != 2 && tableId != 3) || workSheet.Cells[i, 2].Value == null) continue;

                    var name = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : "";
                    var responsible = workSheet.Cells[i, 3].Value != null ? workSheet.Cells[i, 3].Value.ToString() : "";
                    var address = workSheet.Cells[i, 4].Value != null ? workSheet.Cells[i, 4].Value.ToString() : "";
                    var amount = workSheet.Cells[i, 5].Value != null ? Convert.ToDecimal(workSheet.Cells[i, 5].Value.ToString()) : 0;
                    var priceOfContract = workSheet.Cells[i, 6].Value != null ? Convert.ToDecimal(workSheet.Cells[i, 6].Value.ToString()) : 0;
                    var comments = workSheet.Cells[i, 7].Value != null ? workSheet.Cells[i, 7].Value.ToString() : "";
                    var publishDate = workSheet.Cells[i, 8].Value != null ? Convert.ToDateTime(workSheet.Cells[i, 8].Value.ToString()) : DateTime.MinValue;
                    var endDate = workSheet.Cells[i, 9].Value != null ? Convert.ToDateTime(workSheet.Cells[i, 9].Value.ToString()) : DateTime.MinValue;
                    var auctionDate = workSheet.Cells[i, 10].Value != null ? Convert.ToDateTime(workSheet.Cells[i, 10].Value.ToString()) : DateTime.MinValue;
                    var partner = workSheet.Cells[i, 11].Value != null ? workSheet.Cells[i, 11].Value.ToString() : "";
                    var amountOfContract = workSheet.Cells[i, 12].Value != null ? Convert.ToDecimal(workSheet.Cells[i, 12].Value.ToString()) : 0;
                    var dateOfContract = workSheet.Cells[i, 13].Value != null ? Convert.ToDateTime(workSheet.Cells[i, 13].Value.ToString()) : DateTime.MinValue;
                    var stage = workSheet.Cells[i, 14].Value != null ? workSheet.Cells[i, 14].Value.ToString() : "";
                    var sourceOfFinancing = workSheet.Cells[i, 16].Value != null ? workSheet.Cells[i, 16].Value.ToString() : "";
                    var buildingObject = new BuildingObject
                    {
                        Name = name,
                        Address = address,
                        Responsible = responsible,
                        Amount = amount,
                        PriceOfContract = priceOfContract,
                        Comments = comments,
                        PublishDate = publishDate,
                        EndDate = endDate,
                        AuctionDate = auctionDate,
                        Partner = partner,
                        AmountOfContract = amountOfContract,
                        DateOfContract = dateOfContract,
                        Stage = stage,
                        SourceOfFinancing = sourceOfFinancing,
                        LastUpDateTime = DateTime.Now,
                        TypeOfTable = Convert.ToInt32(tableId)
                    };
                    _context.BuildingObject.Add(buildingObject);
                }
                _context.SaveChanges();
            }
            stream.Dispose();
            return RedirectToAction("Index");
        }
    }
}
