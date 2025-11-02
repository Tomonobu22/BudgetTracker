using BudgetTracker.DTOs;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class InvestmentController : Controller
    {
        private readonly IInvestmentAppService _investmentAppService;
        private readonly ITagAppService _tagAppService;

        public InvestmentController(IInvestmentAppService investmentAppService, ITagAppService tagAppService)
        {
            _investmentAppService = investmentAppService;
            _tagAppService = tagAppService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Investment
        public async Task<IActionResult> Index()
        {
            var investments = await _investmentAppService.GetAllByUserAsync(CurrentUserId);
            var types = investments.Select(i => i.Tag?.Name).Distinct().ToList();
            ViewBag.Types = types;
            return View(investments);
        }

        // GET: Filtered Investment
        public async Task<IActionResult> Filter(string? type, DateTime? startDate, DateTime? endDate)
        {
            var investments = await _investmentAppService.GetAllByUserAsync(CurrentUserId);
            if (!string.IsNullOrEmpty(type))
            {
                investments = investments.Where(i => i.Tag != null && i.Tag.Name.ToLower() == type.ToLower());
            }
            if (startDate.HasValue)
            {
                investments = investments.Where(i => i.DateInvested >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                investments = investments.Where(i => i.DateInvested <= endDate.Value);
            }
            return PartialView("_InvestmentTablePartial", investments);
        }


        // GET: Investment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value, CurrentUserId);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // GET: Investment/Create
        public IActionResult Create()
        {
            var tags = _tagAppService.GetAllTagsAsync("Investment", CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            return View();
        }

        // POST: Investment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TagId,Amount,DateInvested,CurrentValue")] InvestmentDto investment, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = "Investment"
                };
                var newId = await _tagAppService.CreateAsync(newTag, CurrentUserId);
                investment.TagId = newId;
            }

            if (!ModelState.IsValid)
            {
                return View(investment);
            }
            await _investmentAppService.CreateAsync(investment, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Investment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value, CurrentUserId);
            if (investment == null)
            {
                return NotFound();
            }
            var tags = _tagAppService.GetAllTagsAsync("Investment", CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            return View(investment);
        }

        // POST: Investment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TagId,Amount,DateInvested,CurrentValue")] InvestmentDto investment)
        {
            if (id != investment.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(investment);
            }

            await _investmentAppService.UpdateAsync(investment, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Investment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value, CurrentUserId);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // POST: Investment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _investmentAppService.DeleteAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        private bool InvestmentExists(int id)
        {
            return _investmentAppService.GetByIdAsync(id, CurrentUserId).Result != null;
        }
    }
}
