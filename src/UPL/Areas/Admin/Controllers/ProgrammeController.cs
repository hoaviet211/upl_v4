using Microsoft.AspNetCore.Mvc;
using UPL.Domain.Entities;
using UPL.Infrastructure.Services;

namespace UPL.Areas.Admin.Controllers;

[Area("Admin")]
public class ProgrammeController : Controller
{
    private readonly IProgrammeService _programmeService;
    public ProgrammeController(IProgrammeService programmeService) { _programmeService = programmeService; }

    public async Task<IActionResult> Index()
        => View(await _programmeService.ListAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Programme model)
    {
        if (!ModelState.IsValid) return View(model);
        await _programmeService.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _programmeService.GetAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Programme model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _programmeService.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _programmeService.GetAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _programmeService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
