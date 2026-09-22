using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Implementations;
using BudgetTracker.Core.Services.Interfaces;
using BudgetTracker.Core.DTOs;
using AutoMapper;
using Moq;
using Xunit;
using BudgetTracker.Core.Enums;

namespace BudgetTracker.Core.Tests.Services
{
    public class IncomeAppServiceTest
    {

        private readonly Mock<IIncomeRepository> _incomeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;

        public IncomeAppServiceTest()
        {
            _incomeRepositoryMock = new Mock<IIncomeRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
        }

        private IncomeAppService CreateService()
        {
            return new IncomeAppService(_incomeRepositoryMock.Object, _mapperMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task GetAllIncomesForUserId_Returns_OkResult()
        {
            var userId = (new Guid()).ToString();
            IEnumerable<Income> incomes = new List<Income>
            {
                new() { Id = 1, UserId = userId, TagId = 1, Tag = new Tag { Context = RecordType.Income , Id = 1 , Name = "Salary" , UserId = userId } , Amount = 1000, Description = "Job", DateReceived = DateTime.UtcNow },
                new() { Id = 2, UserId = userId, TagId = 2, Tag = new Tag { Context = RecordType.Income , Id = 2 , Name = "Freelance" , UserId = userId } , Amount = 500, Description = "Freelance", DateReceived = DateTime.UtcNow }
            };


            // Setup the mock repository to return the fake data
            _incomeRepositoryMock.Setup(repo => repo.GetAllByUserAsync(userId))
                .ReturnsAsync(incomes);

            _mapperMock.Setup(m => m.Map<IEnumerable<IncomeDto>>(It.IsAny<IEnumerable<Income>>()))
                .Returns((IEnumerable<Income> source) => source.Select(i => new IncomeDto
                {
                    Id = i.Id,
                    TagId = i.TagId,
                    Tag = new Tag { Context = i.Tag.Context, Id = i.Tag.Id, Name = i.Tag.Name, UserId = i.Tag.UserId },
                    Amount = i.Amount,
                    Description = i.Description,
                    DateReceived = i.DateReceived
                }));

            var service = CreateService();

            var result = await service.GetAllByUserAsync(userId);
            var expectedDtos = _mapperMock.Object.Map<IEnumerable<IncomeDto>>(incomes);

            Assert.Equal(expectedDtos.Count(), result.Count());
            Assert.Equal(expectedDtos, result, (expected, actual) =>
                       expected.Id == actual.Id &&
                       expected.TagId == actual.TagId &&
                       expected.Tag?.Context == actual.Tag?.Context &&
                       expected.Tag?.Id == actual.Tag?.Id &&
                       expected.Tag?.Name == actual.Tag?.Name &&
                       expected.Tag?.UserId == actual.Tag?.UserId &&
                       expected.Amount == actual.Amount &&
                       expected.Description == actual.Description &&
                       expected.DateReceived == actual.DateReceived
            );

            _incomeRepositoryMock.Verify(repo => repo.GetAllByUserAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
