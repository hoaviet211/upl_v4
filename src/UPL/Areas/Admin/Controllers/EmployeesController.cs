using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPL.Common;
using UPL.Data;
using UPL.Domain.Entities;
using UPL.Models.Admin.Employees;

namespace UPL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class EmployeesController : Controller
{
    private static readonly string[] AllExpectedRoleNames = StaffRoleHelper.Roles
        .Concat(new[] { RoleConstants.Student })
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private readonly UplDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(UplDbContext db, ILogger<EmployeesController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] EmployeeFilterInputModel filter)
    {
        await EnsureRolesAsync();

        var staffRoleSet = StaffRoleHelper.CreateRoleSet();

        var query = _db.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => staffRoleSet.Contains(ur.Role.Name)));

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var keyword = filter.SearchTerm.Trim();
            var like = $"%{keyword}%";
            query = query.Where(u => EF.Functions.Like(u.FullName, like) || EF.Functions.Like(u.Email, like));
        }

        if (!string.IsNullOrWhiteSpace(filter.Role) && staffRoleSet.Contains(filter.Role))
        {
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == filter.Role));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = filter.Status switch
            {
                "active" => query.Where(u => u.IsActive),
                "inactive" => query.Where(u => !u.IsActive),
                _ => query
            };
        }

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new EmployeeListItemViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles
                    .Where(ur => staffRoleSet.Contains(ur.Role.Name))
                    .Select(ur => ur.Role.Name)
                    .OrderBy(r => r)
                    .ToList()
            })
            .ToListAsync();

        var viewModel = new EmployeeListViewModel
        {
            Filter = filter,
            Items = items,
            RoleOptions = await GetRoleOptionsAsync(staffRoleSet)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await EnsureRolesAsync();

        var viewModel = new EmployeeFormViewModel
        {
            RoleOptions = await GetRoleOptionsAsync(),
            IsActive = true
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model)
    {
        await EnsureRolesAsync();
        ValidateRoleSelection(model);

        if (!ModelState.IsValid)
        {
            model.RoleOptions = await GetRoleOptionsAsync();
            return View(model);
        }

        var normalizedEmail = model.Email.Trim();
        var duplicate = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (duplicate != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại trong hệ thống");
            model.RoleOptions = await GetRoleOptionsAsync();
            return View(model);
        }

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Email = normalizedEmail,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password!.Trim());

        if (!string.IsNullOrWhiteSpace(model.Phone) ||
            !string.IsNullOrWhiteSpace(model.ZaloNumber) ||
            !string.IsNullOrWhiteSpace(model.Position) ||
            !string.IsNullOrWhiteSpace(model.Note))
        {
            user.StaffProfile = new StaffProfile
            {
                Phone = model.Phone?.Trim(),
                ZaloNumber = model.ZaloNumber?.Trim(),
                Position = model.Position?.Trim(),
                Note = model.Note?.Trim()
            };
        }

        await AttachRolesAsync(user, model.SelectedRoles);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = "Đã tạo thành viên mới thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        await EnsureRolesAsync();
        var staffRoleSet = StaffRoleHelper.CreateRoleSet();

        var user = await _db.Users
            .AsSplitQuery()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.StaffProfile)
            .FirstOrDefaultAsync(u => u.Id == id && u.UserRoles.Any(ur => staffRoleSet.Contains(ur.Role.Name)));

        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeeFormViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            Phone = user.StaffProfile?.Phone,
            ZaloNumber = user.StaffProfile?.ZaloNumber,
            Position = user.StaffProfile?.Position,
            Note = user.StaffProfile?.Note,
            SelectedRoles = user.UserRoles
                .Where(ur => staffRoleSet.Contains(ur.Role.Name))
                .Select(ur => ur.Role.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            RoleOptions = await GetRoleOptionsAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await EnsureRolesAsync();
        ValidateRoleSelection(model);

        if (!ModelState.IsValid)
        {
            model.RoleOptions = await GetRoleOptionsAsync();
            return View(model);
        }

        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.StaffProfile)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        var normalizedEmail = model.Email.Trim();
        var duplicate = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.Id != id);
        if (duplicate != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã được dùng cho tài khoản khác");
            model.RoleOptions = await GetRoleOptionsAsync();
            return View(model);
        }

        user.FullName = model.FullName.Trim();
        user.Email = normalizedEmail;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password.Trim());
        }

        if (user.StaffProfile == null &&
            (!string.IsNullOrWhiteSpace(model.Phone) ||
             !string.IsNullOrWhiteSpace(model.ZaloNumber) ||
             !string.IsNullOrWhiteSpace(model.Position) ||
             !string.IsNullOrWhiteSpace(model.Note)))
        {
            user.StaffProfile = new StaffProfile
            {
                Phone = model.Phone?.Trim(),
                ZaloNumber = model.ZaloNumber?.Trim(),
                Position = model.Position?.Trim(),
                Note = model.Note?.Trim()
            };
        }
        else if (user.StaffProfile != null)
        {
            user.StaffProfile.Phone = model.Phone?.Trim();
            user.StaffProfile.ZaloNumber = model.ZaloNumber?.Trim();
            user.StaffProfile.Position = model.Position?.Trim();
            user.StaffProfile.Note = model.Note?.Trim();
        }

        await UpdateRolesAsync(user, model.SelectedRoles);

        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = "Đã cập nhật thông tin thành viên.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = user.IsActive
            ? "Đã kích hoạt tài khoản."
            : "Đã vô hiệu hóa tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateRoleSelection(EmployeeFormViewModel model)
    {
        if (model.SelectedRoles == null || !model.SelectedRoles.Any())
        {
            ModelState.AddModelError(nameof(model.SelectedRoles), "Chọn ít nhất một role cho thành viên");
        }
    }

    private async Task AttachRolesAsync(User user, IEnumerable<string>? selectedRoles)
    {
        if (selectedRoles == null) return;

        var targetSet = new HashSet<string>(selectedRoles, StringComparer.OrdinalIgnoreCase);
        if (targetSet.Count == 0) return;

        var roles = await _db.Roles
            .Where(r => targetSet.Contains(r.Name))
            .ToListAsync();

        foreach (var role in roles)
        {
            user.UserRoles.Add(new UserRole { User = user, Role = role, RoleId = role.Id });
        }
    }

    private async Task UpdateRolesAsync(User user, IEnumerable<string>? selectedRoles)
    {
        var targetRoles = new HashSet<string>(selectedRoles ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        var employeeRoleSet = StaffRoleHelper.CreateRoleSet();

        var toRemove = user.UserRoles
            .Where(ur => employeeRoleSet.Contains(ur.Role.Name) && !targetRoles.Contains(ur.Role.Name))
            .ToList();
        foreach (var remove in toRemove)
        {
            user.UserRoles.Remove(remove);
        }

        var existing = user.UserRoles
            .Where(ur => employeeRoleSet.Contains(ur.Role.Name))
            .Select(ur => ur.Role.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = targetRoles.Except(existing, StringComparer.OrdinalIgnoreCase).ToList();
        if (missing.Count == 0) return;

        var roles = await _db.Roles.Where(r => missing.Contains(r.Name)).ToListAsync();
        foreach (var role in roles)
        {
            user.UserRoles.Add(new UserRole { User = user, Role = role, RoleId = role.Id, UserId = user.Id });
        }
    }

    private async Task EnsureRolesAsync()
    {
        var existing = await _db.Roles.AsNoTracking()
            .Where(r => AllExpectedRoleNames.Contains(r.Name))
            .Select(r => r.Name)
            .ToListAsync();

        var comparer = StringComparer.OrdinalIgnoreCase;
        var toAdd = AllExpectedRoleNames.Where(r => !existing.Contains(r, comparer)).ToList();
        if (toAdd.Count == 0) return;

        foreach (var name in toAdd)
        {
            _db.Roles.Add(new Role { Name = name });
        }

        await _db.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<EmployeeRoleOption>> GetRoleOptionsAsync(HashSet<string>? employeeRoleSet = null)
    {
        employeeRoleSet ??= StaffRoleHelper.CreateRoleSet();

        return await _db.Roles.AsNoTracking()
            .Where(r => employeeRoleSet.Contains(r.Name))
            .OrderBy(r => r.Name)
            .Select(r => new EmployeeRoleOption
            {
                Name = r.Name,
                DisplayName = r.Name
            })
            .ToListAsync();
    }
}
