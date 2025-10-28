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
    public class ExpenseController : Controller
    {
        private readonly IExpenseAppService _expenseAppService;

        public ExpenseController(IExpenseAppService expenseAppService)
        {
            _expenseAppService = expenseAppService;
        }

        // GET: Expense
        public async Task<IActionResult> Index()
        {
            return View(await _expenseAppService.GetAllAsync());
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Expense/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Category,Description,Amount,DateIncurred")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                await _expenseAppService.AddAsync(expense);
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST: Expense/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,Description,Amount,DateIncurred")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _expenseAppService.UpdateAsync(expense);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
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
            return View(expense);
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value);      
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _expenseAppService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _expenseAppService.GetByIdAsync(id).Result != null;
        }
    }
}
