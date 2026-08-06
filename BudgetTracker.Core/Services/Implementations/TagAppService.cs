using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Core.Services.Implementations
{
    public class TagAppService : ITagAppService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagAppService> _logger;
        private readonly ICacheService _cacheService;

        public TagAppService(ITagRepository tagRepository, 
                                IMapper mapper,
                                ILogger<TagAppService> logger,
                                ICacheService cacheService)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task RemoveTagAsync(int tagId, string userId)
        {
            // Get the tag to find its context before deletion
            var tag = await _tagRepository.GetTagByIdAsync(tagId);
            if (tag == null || tag.UserId != userId)
            {
                throw new KeyNotFoundException("Tag not found");
            }

            await _tagRepository.RemoveTagAsync(tagId, userId);
            // Invalidate cache for this context
            _cacheService.Remove(CacheKeys.GetTagsByContextKey(userId, tag.Context));
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync(RecordType context, string userId)
        {
            var cacheKey = CacheKeys.GetTagsByContextKey(userId, context);
            var cachedTags = _cacheService.Get<IEnumerable<TagDto>>(cacheKey);
            if (cachedTags != null)
            {
                _logger.LogDebug($"Cache hit {cacheKey}");
                return cachedTags;
            }

            var tags = await _tagRepository.GetAllTagsAsync(context, userId);
            var result = _mapper.Map<IEnumerable<TagDto>>(tags);

            // Cache the result for 10 minutes
            _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            _logger.LogDebug($"Cache set {cacheKey}");

            return result;
        }
        public async Task<int> CreateAsync(TagDto tagDto, string userId)
        {
            // First check if it's not a existing tag
            var currentTags = await GetAllTagsAsync(tagDto.Context, userId);
            var foundTag = currentTags.FirstOrDefault(t => t.Name.Equals(tagDto.Name, StringComparison.OrdinalIgnoreCase));

            if (foundTag != null)
            {
                return foundTag.Id;
            }

            var tag = _mapper.Map<Tag>(tagDto);
            tag.UserId = userId;
            await _tagRepository.CreateAsync(tag);
            _cacheService.Remove(CacheKeys.GetTagsByContextKey(userId, tagDto.Context)); // Invalidate cache for this context
            return tag.Id;
        }
        public async Task<int> UpdateAsync(TagDto tagDto, string userId)
        {
            var tag = _mapper.Map<Tag>(tagDto);
            tag.UserId = userId;
            await _tagRepository.UpdateAsync(tag);

            // Invalidate cache for this context
            // Context cannot be changed, so we can use the context from the DTO
            _cacheService.Remove(CacheKeys.GetTagsByContextKey(userId, tagDto.Context)); 
            return tag.Id;
        }
        public async Task<TagDto> GetTagByIdAsync(int tagId, string userId)
        {
            var tag = await _tagRepository.GetTagByIdAsync(tagId);
            if (tag == null || tag.UserId != userId)
            {
                throw new KeyNotFoundException("Tag not found");
            }
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<PagedResultDto<TagDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, string? searchTerm = null, RecordType? context = RecordType.Empty)
        {
            var tag = _tagRepository.Query(userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                tag = tag.Where(t => t.Name.Contains(searchTerm));
            }
            if (context != null && context != RecordType.Empty)
            {
                tag = tag.Where(t => t.Context == context);
            }

            var totalCount = await tag.CountAsync();

            var items = await tag
                .OrderByDescending(t => t.Context).ThenBy(t => t.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => _mapper.Map<TagDto>(t))
                .ToListAsync();

            return new PagedResultDto<TagDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                TotalAmount = 0,
                TotalCount = totalCount,
                PageSize = request.PageSize
            };
        }
    }
}
