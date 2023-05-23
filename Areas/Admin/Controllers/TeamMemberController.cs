using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.Areas.Admin.ViewModels;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities.Constants;
using WebFrontToBack.Utilities.Extensions;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamMemberController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ICollection<TeamMember> members = await _context.TeamMembers.ToListAsync();
            return View(members);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamMemberVM member)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!member.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", $"{member.Photo.FileName} {Messages.FileTypeMustBeImage}");
                return View();
            }
            if (!member.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", $"{member.Photo.FileName} - {Messages.FileSizeMustBe200KB}");
                return View();
            }

            string root = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img");
            string fileName = await member.Photo.SaveAsync(root);
            TeamMember teamMember = new TeamMember()
            {
                FullName = member.FullName,
                ImagePath = fileName,
                Profession = member.Profession
            };
            await _context.TeamMembers.AddAsync(teamMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            TeamMember existingMember = _context.TeamMembers.Find(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            UpdateTeamMemberVM teamMemberVM = new UpdateTeamMemberVM()
            {
                FullName = existingMember.FullName,
                Profession = existingMember.Profession,
               
            };

            return View(teamMemberVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateTeamMemberVM teamMemberVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            TeamMember existingMember = await _context.TeamMembers.FindAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }
            if (teamMemberVM.Photo != null)
            {
                if (!teamMemberVM.Photo.CheckContentType("image/"))
                {
                    ModelState.AddModelError("Photo", $"{teamMemberVM.Photo.FileName} must be an image type");
                    return View();
                }

                if (!teamMemberVM.Photo.CheckFileSize(1500))
                {
                    ModelState.AddModelError("Photo", $"{teamMemberVM.Photo.FileName} file must be less than 200kb in size");
                    return View();
                }

                string root = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img");

                string existingImagePath = Path.Combine(root, existingMember.ImagePath);
                if (System.IO.File.Exists(existingImagePath))
                {
                    System.IO.File.Delete(existingImagePath);
                }

                string fileName = await teamMemberVM.Photo.SaveAsync(root);
                existingMember.ImagePath = fileName;
            }

            existingMember.FullName = teamMemberVM.FullName;
            existingMember.Profession = teamMemberVM.Profession;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            TeamMember teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null) return NotFound();
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", teamMember.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
