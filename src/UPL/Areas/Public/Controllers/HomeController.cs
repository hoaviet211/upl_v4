using Microsoft.AspNetCore.Mvc;

namespace UPL.Areas.Public.Controllers;

[Area("Public")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}

