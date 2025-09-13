using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using UPL.Data;
using UPL.Domain.Entities;
using UPL.Models.Auth;

namespace UPL.Controllers;

public class AccountController : Controller
{
    private readonly UplDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AccountController(UplDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalized = model.UserNameOrEmail.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);

        // Tránh lộ thông tin: luôn trả về cùng một thông báo lỗi cho mọi trường hợp sai
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng");
            return View(model);
        }

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng");
            return View(model);
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            // Optional: rehash to stronger work factor
            user.PasswordHash = _hasher.HashPassword(user, model.Password);
            await _db.SaveChangesAsync();
        }

        var roleNames = user.UserRoles.Select(ur => ur.Role.Name).Distinct().ToList();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roleNames.Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        if (roleNames.Contains("Admin"))
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        if (roleNames.Contains("Staff"))
            return RedirectToAction("Index", "Home", new { area = "Staff" });
        if (roleNames.Contains("Student"))
            return RedirectToAction("Index", "Home", new { area = "Student" });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET fallback for existing anchor in layout (can change to form POST later)
    [HttpGet]
    public async Task<IActionResult> Logout(bool go = true)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Denied()
    {
        return View();
    }
}
