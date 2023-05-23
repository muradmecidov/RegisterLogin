using Microsoft.AspNetCore.Mvc;

namespace WebFrontToBack.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
