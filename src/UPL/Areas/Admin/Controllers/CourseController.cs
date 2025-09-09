using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UPL.Domain.Entities;
using UPL.Infrastructure.Services;

namespace UPL.Areas.Admin.Controllers;

[Area("Admin")]
public class CourseController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IProgrammeService _programmeService;
    public CourseController(ICourseService courseService, IProgrammeService programmeService)
    {
        _courseService = courseService;
        _programmeService = programmeService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _courseService.ListWithProgrammeAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        var programmes = await _programmeService.ListAsync();
        ViewBag.ProgrammeId = new SelectList(programmes, nameof(Programme.Id), nameof(Programme.ProgrammeName));
        return View(new Course());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course model)
    {
        if (!ModelState.IsValid)
        {
            var programmes = await _programmeService.ListAsync();
            ViewBag.ProgrammeId = new SelectList(programmes, nameof(Programme.Id), nameof(Programme.ProgrammeName), model.ProgrammeId);
            return View(model);
        }
        await _courseService.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _courseService.GetAsync(id);
        if (entity == null) return NotFound();
        var programmes = await _programmeService.ListAsync();
        ViewBag.ProgrammeId = new SelectList(programmes, nameof(Programme.Id), nameof(Programme.ProgrammeName), entity.ProgrammeId);
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            var programmes = await _programmeService.ListAsync();
            ViewBag.ProgrammeId = new SelectList(programmes, nameof(Programme.Id), nameof(Programme.ProgrammeName), model.ProgrammeId);
            return View(model);
        }
        await _courseService.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _courseService.GetWithProgrammeAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _courseService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
