using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Controllers
{
    public class InvestmentController : Controller
    {
        private readonly IInvestmentAppService _investmentAppService;

        public InvestmentController(IInvestmentAppService investmentAppService)
        {
            _investmentAppService = investmentAppService;
        }

        // GET: Investment
        public async Task<IActionResult> Index()
        {
            return View(await _investmentAppService.GetAllAsync());
        }

        // GET: Investment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value);
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
        public async Task<IActionResult> Create([Bind("Id,Type,Amount,DateInvested,CurrentValue")] Investment investment)
        {
            if (ModelState.IsValid)
            {
                await _investmentAppService.AddAsync(investment);
                return RedirectToAction(nameof(Index));
            }
            return View(investment);
        }

        // GET: Investment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Amount,DateInvested,CurrentValue")] Investment investment)
        {
            if (id != investment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _investmentAppService.UpdateAsync(investment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvestmentExists(investment.Id))
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
            return View(investment);
        }

        // GET: Investment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _investmentAppService.GetByIdAsync(id.Value);
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
            await _investmentAppService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool InvestmentExists(int id)
        {
            return _investmentAppService.GetByIdAsync(id).Result != null;
        }
    }
}
