using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.Areas.Admin.ViewModels;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities.Constants;
using WebFrontToBack.Utilities.Extensions;

namespace WebFrontToBack.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]

public class ServicesController : Controller
{
    private readonly AppDbContext _context;
    private List<Category> _categories;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IWebHostEnvironment _enviroment;
    private string _errorMessages;
    public ServicesController(AppDbContext context, IWebHostEnvironment enviroment)
    {
        _context = context;
        _categories = _context.Categories.ToList();
        _enviroment = enviroment;
    }

    public async Task<IActionResult> Index()
    {
        return View(

            await _context.Services
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.Id)
            .Take(8)
            .Include(s => s.Category)
            .Include(s => s.ServiceImages)
            .ToListAsync()
            );
    }

    public async Task<IActionResult> Create()
    {
        CreateServiceVm createServiceVm = new CreateServiceVm()
        {
            Categories = _categories
        };
        return View(createServiceVm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateServiceVm serviceVm)
    {
        serviceVm.Categories = _categories;
        if (!ModelState.IsValid) return View(serviceVm);
        if (!CheckPhoto(serviceVm.Photos)) ModelState.AddModelError("Photos", _errorMessages);
        string rootPath = Path.Combine(_enviroment.WebRootPath, "assets", "img");
        List<ServiceImage> images = await CreateFileAndGetServiceImages(serviceVm.Photos, rootPath);
        Service service = new Service()
        {
            Name = serviceVm.Name,
            CategoryId = serviceVm.CategoryId,
            Description = serviceVm.Description,
            Price = serviceVm.Price,
            ServiceImages = images
        };
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<ServiceImage>> CreateFileAndGetServiceImages(List<IFormFile> photos, string rootPath)
    {
        List<ServiceImage> images = new List<ServiceImage>();
        foreach (var photo in photos)
        {
            string fileName = await photo.SaveAsync(rootPath);
            ServiceImage serviceImage = new ServiceImage() { Path = fileName };
            if (!images.Any(i => i.IsActive))
            {
                serviceImage.IsActive = true;
            }
            images.Add(serviceImage);
        }

        return images;
    }

    private bool CheckPhoto(List<IFormFile> photos)
    {
        foreach (var photo in photos)
        {
            if (!photo.CheckContentType("image/"))
            {
                _errorMessages = $"{photo.FileName} - {Messages.FileTypeMustBeImage}";
                return false;
            }
            if (!photo.CheckFileSize(200))
            {
                _errorMessages = $"{photo.FileName} - {Messages.FileSizeMustBe200KB}";
                return false;
            }
        }
        return true;
    }



    public async Task<IActionResult> Delete(int id)
    {
        Service services = await _context.Services.FindAsync(id);
        if (services == null) return NotFound();
        foreach (var serviceImage in services.ServiceImages)
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", serviceImage.Path);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        _context.Services.Remove(services);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
