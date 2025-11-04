using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Enums;
using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagAppService _tagAppService;
        private readonly IIncomeAppService _incomeAppService;
        private readonly IExpenseAppService _expenseAppService;
        private readonly IInvestmentAppService _investmentAppService;

        public TagController(ITagAppService tagAppService, IIncomeAppService incomeAppService, IExpenseAppService expenseAppService, IInvestmentAppService investmentAppService, IMapper mapper)
        {
            _tagAppService = tagAppService;
            _incomeAppService = incomeAppService;
            _expenseAppService = expenseAppService;
            _investmentAppService = investmentAppService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Tag
        public async Task<IActionResult> Index()
        {
            var tags = await _tagAppService.GetAllTagsAsync(RecordType.Empty, CurrentUserId);
            return View(tags);
        }

        // GET: Filtered Tag
        public async Task<IActionResult> Filter(RecordType? context, string? tagName)
        {
            var tags = await _tagAppService.GetAllTagsAsync(RecordType.Empty, CurrentUserId);
            if (context != null && context != RecordType.Empty)
            {
                tags = tags.Where(t => t.Context == context);
            }
            if (!string.IsNullOrEmpty(tagName))
            {
                tags = tags.Where(t => t.Name.Contains(tagName, StringComparison.OrdinalIgnoreCase));
            }
            return PartialView("_TagTablePartial", tags);
        }

        // GET: Tag/Create
        public IActionResult Create()
        {
            List<RecordType> contexts = new List<RecordType> { RecordType.Income, RecordType.Expense, RecordType.Investment };
            ViewBag.Contexts = new SelectList(contexts);
            return View();
        }

        // POST: Tag/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Context")] TagDto tag)
        {
            if (ModelState.IsValid)
            {
                await _tagAppService.CreateAsync(tag, CurrentUserId);
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tag/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _tagAppService.GetTagByIdAsync(id.Value, CurrentUserId);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tag/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Context")] TagDto tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tagAppService.UpdateAsync(tag, CurrentUserId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tag/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _tagAppService.GetTagByIdAsync(id.Value, CurrentUserId);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _tagAppService.GetTagByIdAsync(id, CurrentUserId);
            if (tag != null)
            {
                // First check if there are no incomes or expenses or investments associated with this tag
                var hasIncomes = await _incomeAppService.HasIncomesWithTagAsync(id);
                var hasExpenses = await _expenseAppService.HasExpensesWithTagAsync(id);
                var hasInvestments = await _investmentAppService.HasInvestmentsWithTagAsync(id);

                if (hasIncomes || hasExpenses || hasInvestments)
                {
                    ModelState.AddModelError(string.Empty, "Tag cannot be deleted as it is associated with existing records.");
                    return View(tag);
                }
                else
                {
                    await _tagAppService.RemoveTagAsync(id, CurrentUserId);
                }
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool TagExists(int id)
        {
            return _tagAppService.GetTagByIdAsync(id, CurrentUserId) != null;
        }
    }
}
