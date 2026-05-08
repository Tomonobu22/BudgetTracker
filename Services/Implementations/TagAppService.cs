using AutoMapper;
using BudgetTracker.DTOs;
using BudgetTracker.Enums;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class TagAppService : ITagAppService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        public TagAppService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task RemoveTagAsync(int tagId, string userId)
        {
            await _tagRepository.RemoveTagAsync(tagId, userId);
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync(RecordType context, string userId)
        {
            var tags = await _tagRepository.GetAllTagsAsync(context, userId);
            return _mapper.Map<IEnumerable<TagDto>>(tags);
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
            return tag.Id;
        }
        public async Task<int> UpdateAsync(TagDto tagDto, string userId)
        {
            var tag = _mapper.Map<Tag>(tagDto);
            tag.UserId = userId;
            await _tagRepository.UpdateAsync(tag);
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
    }
}
