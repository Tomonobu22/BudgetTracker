using BudgetTracker.DTOs;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class InvestmentController : Controller
    {
        private readonly IInvestmentAppService _investmentAppService;

        public InvestmentController(IInvestmentAppService investmentAppService)
        {
            _investmentAppService = investmentAppService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Investment
        public async Task<IActionResult> Index()
        {
            var investments = await _investmentAppService.GetAllByUserAsync(CurrentUserId);
            return View(investments);
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
            return View();
        }

        // POST: Investment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Amount,DateInvested,CurrentValue")] InvestmentDto investment)
        {
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
            return View(investment);
        }

        // POST: Investment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Amount,DateInvested,CurrentValue")] InvestmentDto investment)
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
