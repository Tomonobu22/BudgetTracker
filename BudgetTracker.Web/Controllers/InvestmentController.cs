using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Services.Interfaces;
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
        private readonly IStockPriceAppService _stockPriceAppService;

        public InvestmentController(IInvestmentAppService investmentAppService, 
                                    ITagAppService tagAppService,
                                    IStockPriceAppService stockPriceAppService)
        {
            _investmentAppService = investmentAppService;
            _tagAppService = tagAppService;
            _stockPriceAppService = stockPriceAppService;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Investment
        public async Task<IActionResult> Index(int page = 1)
        {
            // For add new
            var tags = _tagAppService.GetAllTagsAsync(RecordType.Investment, CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            var tickers = await _stockPriceAppService.GetAllAsync();
            ViewBag.Tickers = new SelectList(tickers, "Ticker", "Ticker");

            var investments = await _investmentAppService.GetPagedByUserAsync(CurrentUserId, new PagingRequestDto { PageNumber = page, PageSize = 10 });
            var types = tags.Result.Select(i => i.Name).Distinct().ToList();
            ViewBag.Types = types;
            return View(investments);
        }

        // GET: Filtered Investment
        public async Task<IActionResult> Filter(string? type, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var filter = new InvestmentFilterDto
            {
                Type = type,
                StartDate = startDate,
                EndDate = endDate
            };
            var investments = await _investmentAppService.GetPagedByUserAsync(CurrentUserId, new PagingRequestDto { PageNumber = page, PageSize = 10 }, filter);
            return PartialView("_InvestmentTablePartial", investments);
        }


        // POST: Investment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TagId,Ticker,PurchasePrice,Quantity,DateInvested")] InvestmentDto investment, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = RecordType.Investment
                };
                var newId = await _tagAppService.CreateAsync(newTag, CurrentUserId);
                investment.TagId = newId;
            }

            if (!ModelState.IsValid)
            {
                return View(investment);
            }

            // Add current value to the investment object before updating
            var stockPrice = await _stockPriceAppService.GetByTickerAsync(investment.Ticker);
            if (stockPrice != null)
            {
                investment.CurrentValue = stockPrice.Price * investment.Quantity;
            }
            else
            {
                ModelState.AddModelError("Ticker", "Invalid ticker symbol.");
                return View(investment);
            }

            await _investmentAppService.CreateAsync(investment, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditModal(int? id)
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
            var tags = _tagAppService.GetAllTagsAsync(RecordType.Investment, CurrentUserId);
            ViewBag.Tags = new SelectList(tags.Result, "Id", "Name");
            var tickers = await _stockPriceAppService.GetAllAsync();
            ViewBag.Tickers = new SelectList(tickers, "Ticker", "Ticker");

            return PartialView("_EditModal", investment);
        }

        // POST: Investment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TagId,Ticker,PurchasePrice,Quantity,DateInvested")] InvestmentDto investment, string? newTagName)
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                var newTag = new TagDto
                {
                    Name = newTagName,
                    Context = RecordType.Investment
                };
                var newId = await _tagAppService.CreateAsync(newTag, CurrentUserId);
                investment.TagId = newId;
            }

            if (id != investment.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(investment);
            }

            // Add current value to the investment object before updating
            var stockPrice = await _stockPriceAppService.GetByTickerAsync(investment.Ticker);
            if (stockPrice != null)
            {
                investment.CurrentValue = stockPrice.Price * investment.Quantity;
            }
            else
            {
                ModelState.AddModelError("Ticker", "Invalid ticker symbol.");
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
