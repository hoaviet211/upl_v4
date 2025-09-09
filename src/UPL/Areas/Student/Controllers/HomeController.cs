using Microsoft.AspNetCore.Mvc;

namespace UPL.Areas.Student.Controllers;

[Area("Student")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}

