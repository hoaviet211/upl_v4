using Microsoft.AspNetCore.Mvc;

namespace UPL.Areas.Staff.Controllers;

[Area("Staff")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}

