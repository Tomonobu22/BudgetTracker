using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Enums;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class ImportAppService: IImportAppService
    {
        private readonly IImportRepository _importRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        private const int DailyUploadLimit = 5; // Example limit, can be configured

        public ImportAppService(IImportRepository importRepository, 
            IBlobStorageService blobStorageService, 
            IMapper mapper) 
        {
            _importRepository = importRepository;
            _blobStorageService = blobStorageService;
            _mapper = mapper;
        }

        public async Task<ImportDto> CreateImportAsync(IFormFile file, string userId, CancellationToken cancellationToken)
        {
            var todayUploads = await _importRepository.CountImportsByUserAndDateAsync(userId, DateTime.UtcNow, cancellationToken);
            if (todayUploads >= DailyUploadLimit)
            {
                throw new InvalidOperationException($"Daily upload limit of {DailyUploadLimit} reached.");
            }

            var import = new Import
            {
                FileName = file.FileName,
                BlobName = $"imports/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                UserId = userId,
                Status = ImportStatus.Pending,
                UploadedAt = DateTime.UtcNow
            };

            // Create import entity
            await _importRepository.AddAsync(import);

            // Upload file to blob storage
            try {
                await using var stream = file.OpenReadStream();
                await _blobStorageService.UploadAsync(stream, import.BlobName, file.ContentType, cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle blob storage upload failure
                import.Status = ImportStatus.Failed;
                await _importRepository.UpdateAsync(import);
                throw new Exception("Failed to upload file to blob storage.", ex);
            }

            return _mapper.Map<ImportDto>(import);
        }

        public Task CreateAsync(ImportDto import, string userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ImportDto>> GetAllByUserAsync(string userId)
        {
            var imports = await _importRepository.GetAllByUserAsync(userId); 
            return _mapper.Map<IEnumerable<ImportDto>>(imports);
        }

        public async Task<ImportDto> GetByIdAsync(int id, string userId)
        {
            var import = await _importRepository.GetByIdAsync(id);
            if (import == null || import.UserId != userId)
            {
                throw new KeyNotFoundException("Import not found");
            }
            return _mapper.Map<ImportDto>(import);
        }

        public Task UpdateAsync(ImportDto import, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
