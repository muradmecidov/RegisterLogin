﻿using Microsoft.AspNetCore.Mvc;

namespace WebFrontToBack.ViewComponents
{
    public class FooterViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
