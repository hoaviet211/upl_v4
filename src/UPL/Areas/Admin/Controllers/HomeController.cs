using Microsoft.AspNetCore.Mvc;

namespace UPL.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}

