using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetTracker.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private readonly IImportAppService _importAppService;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly IExpenseAppService _expenseAppService;
        private readonly IIncomeAppService _incomeAppService;
        private readonly IInvestmentAppService _investmentAppService;

        public ImportController(IImportAppService importAppService, IExpenseAppService expenseAppService, IIncomeAppService incomeAppService, IInvestmentAppService investmentAppService) {
            _importAppService = importAppService;
            _expenseAppService = expenseAppService;
            _incomeAppService = incomeAppService;
            _investmentAppService = investmentAppService;
        }
        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);


        public async Task<IActionResult> Index()
        {
            var imports = await _importAppService.GetAllByUserAsync(CurrentUserId);
            return View(imports);
        }

        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile file, RecordType importType, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a CSV file.");
                return View("Index");
            }

            // Check max file size
            if (file.Length > MaxFileSize)
            {
                ModelState.AddModelError("File", "The file size exceeds the maximum limit of 5 MB.");
                return View("Index");
            }

            // Only accept CSV files
            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("File", "Only CSV files are allowed.");
                return View("Index");
            }

            var uploadRequest = new FileUploadRequest
            {
                Content = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            var import = await _importAppService.CreateImportAsync(uploadRequest, importType, CurrentUserId, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = import.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _importAppService.DeleteAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Import/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            try
            {
                var importDto = await _importAppService.GetByIdAsync(id, userId);
                if (importDto == null)
                {
                    return NotFound();
                }

                var viewModel = new ImportDetailsViewModel
                {
                    Import = importDto
                };

                if (importDto.Status == ImportStatus.Completed)
                { 
                    switch (importDto.ImportType)
                    {
                        case RecordType.Expense:
                            viewModel.Expenses = await _expenseAppService.GetExpensesByImportIdAsync(importDto.Id, userId);
                            break;
                        case RecordType.Income:
                            viewModel.Incomes = await _incomeAppService.GetIncomesByImportIdAsync(importDto.Id, userId);
                            break;
                        case RecordType.Investment:
                            viewModel.Investments = await _investmentAppService.GetInvestmentsByImportIdAsync(importDto.Id, userId);
                            break;
                    }
                }
                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
