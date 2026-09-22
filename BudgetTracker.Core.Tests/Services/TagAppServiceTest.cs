using AutoMapper;
using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Helpers;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Implementations;
using BudgetTracker.Core.Services.Interfaces;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudgetTracker.Core.Tests.Services
{
    public class TagAppServiceTest
    {
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILogger<TagAppService>> _loggerMock;

        public TagAppServiceTest() { 
            _tagRepositoryMock = new Mock<ITagRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<TagAppService>>();
        }

        private TagAppService CreateService()
        {
            return new TagAppService(_tagRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task GetAllTagsForUserId_Returns_OkResult()
        {
            var userId = (new Guid()).ToString();

            List<Tag> tags = new List<Tag>
            {
                new Tag { Id = 1, UserId = userId, Name = "Food", Context = RecordType.Expense },
                new Tag { Id = 2, UserId = userId, Name = "Transport", Context = RecordType.Expense }
            };

            // Setup the mock repository to return the fake data
            _tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync(RecordType.Expense, userId))
                .ReturnsAsync(tags);

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<TagDto>>(It.IsAny<IEnumerable<Tag>>()))
                .Returns((IEnumerable<Tag> source) => source.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Context = t.Context
                }));

            var service = CreateService();

            // First clean the cache to ensure the repository is called
            var cacheKey = CacheKeys.GetTagsByContextKey(userId, RecordType.Expense);
            _cacheServiceMock.Setup(cache => cache.Get<IEnumerable<TagDto>>(cacheKey))
                .Returns((IEnumerable<TagDto>)null);

            var result = await service.GetAllTagsAsync(RecordType.Expense, userId);
            var tagsDto = _mapperMock.Object.Map<IEnumerable<TagDto>>(tags);

            Assert.Equal(tagsDto.Count(), result.Count());
            Assert.Equal(tagsDto, result, (expected, actual) =>
                       expected.Id == actual.Id &&
                       expected.Name == actual.Name &&
                       expected.Context == actual.Context
            );

            _tagRepositoryMock.Verify(repo => repo.GetAllTagsAsync(It.IsAny<RecordType>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAllTagsForUserId_WhenTagsIsCached_Returns_OkResult()
        {
            var userId = (new Guid()).ToString();
            List<Tag> tags = new List<Tag>
            {
                new Tag { Id = 1, UserId = userId, Name = "Food", Context = RecordType.Expense },
                new Tag { Id = 2, UserId = userId, Name = "Transport", Context = RecordType.Expense }
            };

            var cacheKey = CacheKeys.GetTagsByContextKey(userId, RecordType.Expense);
            _cacheServiceMock.Setup(cache => cache.Get<List<TagDto>>(cacheKey))
                .Returns(_mapperMock.Object.Map<List<TagDto>>(tags));

            var service = CreateService();

            var result = await service.GetAllTagsAsync(RecordType.Expense, userId);
            var tagsDto = _mapperMock.Object.Map<IEnumerable<TagDto>>(tags);

            Assert.Equal(tagsDto.Count(), result.Count());
            Assert.Equal(tagsDto, result, (expected, actual) =>
                       expected.Id == actual.Id &&
                       expected.Name == actual.Name &&
                       expected.Context == actual.Context
            );

            // Never as the data is already cached, the repository method should not be called
            _tagRepositoryMock.Verify(repo => repo.GetAllTagsAsync(It.IsAny<RecordType>(), It.IsAny<string>()), Times.Never);
        }
    }
}
