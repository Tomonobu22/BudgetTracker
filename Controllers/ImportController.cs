using BudgetTracker.Services.Interfaces;
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

        public ImportController(IImportAppService importAppService) {
            _importAppService = importAppService;
        }
        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
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

            var import = await _importAppService.CreateImportAsync(file, CurrentUserId, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = import.Id });
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
                return View(importDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
