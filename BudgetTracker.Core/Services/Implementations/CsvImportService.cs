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

        async Task<IEnumerable<Expense>> ICsvImportService.ParseAsync(Stream csvStream, string userId, CancellationToken cancellationToken)
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
    }
}
