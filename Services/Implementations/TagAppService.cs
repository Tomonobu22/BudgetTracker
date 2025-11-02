using BudgetTracker.DTOs;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class TagAppService : ITagAppService
    {
        private readonly ITagRepository _tagRepository;
        public TagAppService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task RemoveTagAsync(int tagId, string userId)
        {
            await _tagRepository.RemoveTagAsync(tagId, userId);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(string context, string userId)
        {
            return await _tagRepository.GetAllTagsAsync(context, userId);
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

            var tag = new Tag
            {
                Name = tagDto.Name,
                Context = tagDto.Context,
                UserId = userId
            };
            await _tagRepository.CreateAsync(tag);
            return tag.Id;
        }
    }
}
