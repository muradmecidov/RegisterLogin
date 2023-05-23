using Microsoft.AspNetCore.Mvc;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using WebFrontToBack.Areas.Admin.ViewModels;
using System.Diagnostics.Metrics;
using WebFrontToBack.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class RecentWorkController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecentWorkController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ICollection<RecentWork> recentWorks = await _context.RecentWorks.ToListAsync();
            return View(recentWorks);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecentWorkVM recentWork)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!recentWork.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", $"{recentWork.Photo.FileName} must be image type");
                return View();
            }
            if (!recentWork.Photo.CheckFileSize(1500))
            {
                ModelState.AddModelError("Photo", $"{recentWork.Photo.FileName} file must be size less than 200kb ");
                return View();

            }
            string root = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img");
            string fileName = await recentWork.Photo.SaveAsync(root);
           RecentWork recentwork = new RecentWork()
            {
                Title=recentWork.Title,
                Description=recentWork.Description,
                ImagePath=fileName
            };
            await _context.RecentWorks.AddAsync(recentwork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Update(int id)
        {
            RecentWork existingWork = _context.RecentWorks.Find(id);
            if (existingWork == null)
            {
                return NotFound();
            }

            UpdateRecentWorkVM recentWorkVM = new UpdateRecentWorkVM()
            {
                Title = existingWork.Title,
                Description = existingWork.Description
            };

            return View(recentWorkVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateRecentWorkVM recentWork)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            RecentWork existingWork = await _context.RecentWorks.FindAsync(id);
            if (existingWork == null)
            {
                return NotFound();
            }
            if (recentWork.Photo != null)
            {
                if (!recentWork.Photo.CheckContentType("image/"))
                {
                    ModelState.AddModelError("Photo", $"{recentWork.Photo.FileName} must be an image type");
                    return View();
                }

                if (!recentWork.Photo.CheckFileSize(1500))
                {
                    ModelState.AddModelError("Photo", $"{recentWork.Photo.FileName} file must be less than 200kb in size");
                    return View();
                }

                string root = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img");

                string existingImagePath = Path.Combine(root, existingWork.ImagePath);
                if (System.IO.File.Exists(existingImagePath))
                {
                    System.IO.File.Delete(existingImagePath);
                }

                string fileName = await recentWork.Photo.SaveAsync(root);
                existingWork.ImagePath = fileName;
            }

            existingWork.Title = recentWork.Title;
            existingWork.Description = recentWork.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int Id)
        {
            RecentWork? recentWork = _context.RecentWorks.Find(Id);
            if (recentWork == null)
            {
                return NotFound();
            }
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", recentWork.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.RecentWorks.Remove(recentWork);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
