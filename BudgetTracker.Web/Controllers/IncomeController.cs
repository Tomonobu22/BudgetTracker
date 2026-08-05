using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Services.Implementations;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly IIncomeAppService _incomeAppService;
        private readonly ITagAppService _tagAppService;

        public IncomeController(IIncomeAppService incomeAppService, ITagAppService tagAppService)
        {
            _incomeAppService = incomeAppService;
            _tagAppService = tagAppService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Income
        public async Task<IActionResult> Index(int page = 1)
        {
            // For add new
            var tags = _tagAppService.GetAllTagsAsync(RecordType.Income, CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");

            var incomes = await _incomeAppService.GetPagedByUserAsync(CurrentUserId, new PagingRequestDto { PageNumber = page, PageSize = 10 });
            var source = tags.Result.Select(t => t.Name).Distinct().ToList();
            ViewBag.Sources = source;
            return View(incomes);
        }

        // GET: Filtered Income
        public async Task<IActionResult> Filter(string? source, string? description, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var filter = new IncomeFilterDto
            {
                Source = source,
                Description = description,
                StartDate = startDate,
                EndDate = endDate
            };
            var incomes = await _incomeAppService.GetPagedByUserAsync(CurrentUserId, new PagingRequestDto { PageNumber = page, PageSize = 10 }, filter);
            return PartialView("_IncomeTablePartial", incomes);
        }

        // POST: Income/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TagId,Description,Amount,DateReceived")] IncomeDto income, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = RecordType.Income
                };
                var newId = await _tagAppService.CreateAsync(newTag, CurrentUserId);
                income.TagId = newId;
            }

            if (!ModelState.IsValid)
            {
                return View(income);
            }
            await _incomeAppService.CreateAsync(income, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditModal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeAppService.GetByIdAsync(id.Value, CurrentUserId);
            if (income == null)
            {
                return NotFound();
            }
            var tags = _tagAppService.GetAllTagsAsync(RecordType.Income, CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            return PartialView("_EditModal", income);
        }

        // POST: Income/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TagId,Description,Amount,DateReceived")] IncomeDto dto, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = RecordType.Income
                };
                var newId = await _tagAppService.CreateAsync(newTag, CurrentUserId);
                dto.TagId = newId;
            }

            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _incomeAppService.UpdateAsync(dto, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeAppService.GetByIdAsync(id.Value, CurrentUserId);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        // POST: Income/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _incomeAppService.DeleteAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        private bool IncomeExists(int id)
        {
            return _incomeAppService.GetByIdAsync(id, CurrentUserId).Result != null;
        }
    }
}
