using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UPL.Common;

namespace UPL.Areas.Staff.Controllers;

[Area("Staff")]
[Authorize(Policy = RoleConstants.StaffAccessPolicy)]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}

