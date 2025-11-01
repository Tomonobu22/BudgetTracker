using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IExpenseAppService _expenseAppService;
        private readonly IMapper _mapper;

        public ExpenseController(IExpenseAppService expenseAppService, IMapper mapper)
        {
            _expenseAppService = expenseAppService;
            _mapper = mapper;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Expense
        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseAppService.GetAllByUserAsync(CurrentUserId);
            var categories = expenses.Select(e => e.Tag?.Name).Distinct().ToList();
            ViewBag.Categories = categories;
            return View(expenses);
        }

        // GET: Filtered Expense
        public async Task<IActionResult> Filter(string? category, string? description, DateTime? startDate, DateTime? endDate)
        {
            var expenses = await _expenseAppService.GetAllByUserAsync(CurrentUserId);
            if (!string.IsNullOrEmpty(category))
            {
                expenses = expenses.Where(e => e.Tag != null && e.Tag.Name.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(description))
            {
                expenses = expenses.Where(e => e.Description.Contains(description, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (startDate.HasValue)
            {
                expenses = expenses.Where(e => e.DateIncurred >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                expenses = expenses.Where(e => e.DateIncurred <= endDate.Value).ToList();
            }
            return PartialView("_ExpenseTablePartial", expenses);
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value, CurrentUserId);
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
        public async Task<IActionResult> Create([Bind("Id,Category,Description,Amount,DateIncurred")] ExpenseDto expense)
        {
            if (!ModelState.IsValid)
            {
                return View(expense);
            }
            await _expenseAppService.CreateAsync(expense, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value, CurrentUserId);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,Description,Amount,DateIncurred")] ExpenseDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }
                
            await _expenseAppService.UpdateAsync(dto, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseAppService.GetByIdAsync(id.Value, CurrentUserId);      
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
            await _expenseAppService.DeleteAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }
        
        private bool ExpenseExists(int id)
        {
            return _expenseAppService.GetByIdAsync(id, CurrentUserId).Result != null;
        }
    }
}
