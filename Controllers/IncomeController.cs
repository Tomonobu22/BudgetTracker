using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly IIncomeAppService _incomeAppService;

        public IncomeController(IIncomeAppService incomeAppService)
        {
            _incomeAppService = incomeAppService;
        }

        // GET: Income
        public async Task<IActionResult> Index()
        {
            return View(await _incomeAppService.GetAllAsync());
        }

        // GET: Income/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeAppService.GetByIdAsync(id.Value);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        // GET: Income/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Income/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Source,Amount,DateReceived")] Income income)
        {
            if (ModelState.IsValid)
            {
                await _incomeAppService.AddAsync(income);
                return RedirectToAction(nameof(Index));
            }
            return View(income);
        }

        // GET: Income/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeAppService.GetByIdAsync(id.Value);
            if (income == null)
            {
                return NotFound();
            }
            return View(income);
        }

        // POST: Income/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Source,Amount,DateReceived")] Income income)
        {
            if (id != income.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _incomeAppService.UpdateAsync(income);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomeExists(income.Id))
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
            return View(income);
        }

        // GET: Income/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeAppService.GetByIdAsync(id.Value);
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
            await _incomeAppService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool IncomeExists(int id)
        {
            return _incomeAppService.GetByIdAsync(id).Result != null;
        }
    }
}
