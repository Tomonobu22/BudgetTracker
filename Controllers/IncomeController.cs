using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly IIncomeAppService _incomeAppService;
        private readonly IMapper _mapper;

        public IncomeController(IIncomeAppService incomeAppService, IMapper mapper)
        {
            _incomeAppService = incomeAppService;
            _mapper = mapper;
        }

        // GET: Income
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var incomes = await _incomeAppService.GetAllByUserAsync(userId);
            return View(incomes);
        }

        // GET: Income/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var income = await _incomeAppService.GetByIdAsync(id.Value);
        //    if (income == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(income);
        //}

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
        public async Task<IActionResult> Create([Bind("Id,Source,Amount,DateReceived")] IncomeDto income)
        {
            if (!ModelState.IsValid)
            {
                return View(income);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _incomeAppService.CreateAsync(income, userId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var income = await _incomeAppService.GetByIdAsync(id.Value);
        //    if (income == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(income);
        //}

        // POST: Income/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Source,Amount,DateReceived")] IncomeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var income = await _incomeAppService.GetByIdAsync(id, userId);
            if (income == null) return NotFound();

            _mapper.Map(dto, income); // Update model from DTO
            await _incomeAppService.UpdateAsync(income);
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var income = await _incomeAppService.GetByIdAsync(id.Value);
        //    if (income == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(income);
        //}

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _incomeAppService.GetByIdAsync(id, userId).Result != null;
        }
    }
}
