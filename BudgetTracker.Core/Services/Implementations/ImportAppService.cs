using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;

namespace BudgetTracker.Core.Services.Implementations
{
    public class ImportAppService: IImportAppService
    {
        private readonly IImportRepository _importRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IImportQueueService _importQueueService;
        private readonly IMapper _mapper;
        private const int DailyUploadLimit = 5; // Example limit, can be configured

        public ImportAppService(IImportRepository importRepository, 
            IBlobStorageService blobStorageService, 
            IImportQueueService importQueueService,
            IMapper mapper) 
        {
            _importRepository = importRepository;
            _blobStorageService = blobStorageService;
            _importQueueService = importQueueService;
            _mapper = mapper;
        }

        public async Task<ImportDto> CreateImportAsync(FileUploadRequest request, RecordType importType, string userId, CancellationToken cancellationToken)
        {
            var todayUploads = await _importRepository.CountImportsByUserAndDateAsync(userId, DateTime.UtcNow, cancellationToken);
            if (todayUploads >= DailyUploadLimit)
            {
                throw new InvalidOperationException($"Daily upload limit of {DailyUploadLimit} reached.");
            }

            var import = new Import
            {
                FileName = request.FileName,
                BlobName = $"imports/{Guid.NewGuid()}{Path.GetExtension(request.FileName)}",
                UserId = userId,
                Status = ImportStatus.Pending,
                UploadedAt = DateTime.UtcNow,
                ImportType = importType
            };

            // Create import entity
            await _importRepository.AddAsync(import);

            // Upload file to blob storage
            try {
                await _blobStorageService.UploadAsync(request.Content, import.BlobName, request.ContentType, cancellationToken);
                // Enqueue import for processing
                await _importQueueService.EnqueueImportAsync(import.Id);
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
