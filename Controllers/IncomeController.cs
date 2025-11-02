using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Services.Implementations;
using BudgetTracker.Services.Interfaces;
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
        public async Task<IActionResult> Index()
        {
            var incomes = await _incomeAppService.GetAllByUserAsync(CurrentUserId);
            var source = incomes.Select(i => i.Tag?.Name).Distinct().ToList();
            ViewBag.Sources = source;
            return View(incomes);
        }

        // GET: Filtered Income
        public async Task<IActionResult> Filter(string? source, DateTime? startDate, DateTime? endDate)
        {
            var incomes = await _incomeAppService.GetAllByUserAsync(CurrentUserId);
            if (!string.IsNullOrEmpty(source))
            {
                incomes = incomes.Where(i => i.Tag != null && i.Tag.Name.ToLower() == source.ToLower());
            }
            if (startDate.HasValue)
            {
                incomes = incomes.Where(i => i.DateReceived >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                incomes = incomes.Where(i => i.DateReceived <= endDate.Value);
            }

            return PartialView("_IncomeTablePartial", incomes);
        }

        // GET: Income/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Income/Create
        public IActionResult Create()
        {
            var tags = _tagAppService.GetAllTagsAsync("Income", CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            return View();
        }

        // POST: Income/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TagId,Amount,DateReceived")] IncomeDto income, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = "Income"
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

        // GET: Income/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            var tags = _tagAppService.GetAllTagsAsync("Income", CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            return View(income);
        }

        // POST: Income/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TagId,Amount,DateReceived")] IncomeDto dto)
        {
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
