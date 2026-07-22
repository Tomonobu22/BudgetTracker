using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services.Interfaces;
namespace BudgetTracker.Core.Services.Implementations
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ITagRepository _tagRepository;

        public CsvImportService(IImportRepository importRepository, ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        async Task<IEnumerable<Expense>> ICsvImportService.ParseExpenseAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default)
        {
            var expenses = new List<Expense>();
            using var reader = new StreamReader(csvStream);

            // Skip header
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                    continue;

                // Date,Description,Category,Amount
                var values = line.Split(',');
                if (values.Length < 4)
                    continue;

                // The category is passed as a string, but we need to convert it to a TagId.
                // We'll assume that the category is the name of the tag, and we'll look it up in the database.
                var tagName = values[2];
                var tag = await _tagRepository.GetTagByNameAsync(tagName, RecordType.Expense, userId);

                if (tag == null)
                {
                    // If the tag doesn't exist, we can create it.
                    tag = new Tag
                    {
                        Name = tagName,
                        Context = RecordType.Expense,
                        UserId = userId
                    };
                    await _tagRepository.CreateAsync(tag);
                }

                expenses.Add(new Expense
                {
                    DateIncurred = DateTime.TryParse(values[0], out var date) ? date : DateTime.MinValue,
                    Description = values[1],
                    TagId = tag.Id,
                    Amount = decimal.TryParse(values[3], out var amount) ? amount : 0,
                    UserId = userId
                }
                );
            }
            return expenses;
        }

        async Task<IEnumerable<Income>> ICsvImportService.ParseIncomeAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default)
        {
            var incomes = new List<Income>();
            using var reader = new StreamReader(csvStream);

            // Skip header
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                    continue;

                // Date,Description,Category,Amount
                var values = line.Split(',');
                if (values.Length < 4)
                    continue;

                // The category is passed as a string, but we need to convert it to a TagId.
                // We'll assume that the category is the name of the tag, and we'll look it up in the database.
                var tagName = values[2];
                var tag = await _tagRepository.GetTagByNameAsync(tagName, RecordType.Income, userId);

                if (tag == null)
                {
                    // If the tag doesn't exist, we can create it.
                    tag = new Tag
                    {
                        Name = tagName,
                        Context = RecordType.Income,
                        UserId = userId
                    };
                    await _tagRepository.CreateAsync(tag);
                }

                incomes.Add(new Income
                {
                    DateReceived = DateTime.TryParse(values[0], out var date) ? date : DateTime.MinValue,
                    Description = values[1],
                    TagId = tag.Id,
                    Amount = decimal.TryParse(values[3], out var amount) ? amount : 0,
                    UserId = userId
                }
                );
            }
            return incomes;
        }

        async Task<IEnumerable<Investment>> ICsvImportService.ParseInvestmentAsync(Stream csvStream, string userId, CancellationToken cancellationToken = default)
        {
            var investments = new List<Investment>();
            using var reader = new StreamReader(csvStream);

            // Skip header
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                    continue;

                // Date,Description,Category,Amount
                var values = line.Split(',');
                if (values.Length < 3)
                    continue;

                // The category is passed as a string, but we need to convert it to a TagId.
                // We'll assume that the category is the name of the tag, and we'll look it up in the database.
                var tagName = values[1];
                var tag = await _tagRepository.GetTagByNameAsync(tagName, RecordType.Investment, userId);

                if (tag == null)
                {
                    // If the tag doesn't exist, we can create it.
                    tag = new Tag
                    {
                        Name = tagName,
                        Context = RecordType.Investment,
                        UserId = userId
                    };
                    await _tagRepository.CreateAsync(tag);
                }

                investments.Add(new Investment
                {
                    DateInvested = DateTime.TryParse(values[0], out var date) ? date : DateTime.MinValue,
                    TagId = tag.Id,
                    Amount = decimal.TryParse(values[2], out var amount) ? amount : 0,
                    UserId = userId
                }
                );
            }
            return investments;
        }

    }
}
