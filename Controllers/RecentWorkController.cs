using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;

namespace WebFrontToBack.Controllers
{
    public class RecentWorkController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public RecentWorkController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            List<WorkCategory> workCategories = await _appDbContext.WorkCategories.ToListAsync();
            return View(workCategories);
        }
    }
}
