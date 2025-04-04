using ClickCounter.Application.Services.Counter.DTOs;
using ClickCounter.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClickCounter.Application.Services.Counter;

public interface ICounterService {
    Task<List<CounterDto>> GetAllAsync();
}

public sealed class CounterService : ICounterService {
    private readonly IDbContextFactory<ClickCounterDbContext> _dbContextFactory;
    private readonly CancellationToken _cancellationToken;

    public CounterService(IDbContextFactory<ClickCounterDbContext> dbContextFactory, CancellationToken cancellationToken) {
        _dbContextFactory = dbContextFactory;
        _cancellationToken = cancellationToken;
    }

    public async Task<List<CounterDto>> GetAllAsync() {
        await using ClickCounterDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(_cancellationToken);

        return await dbContext.Counters.Where(counter => counter.Trash == 0).Select(counter => new CounterDto {
            CounterId = counter.CounterId,
            Name = counter.Name ?? string.Empty,
            Count = counter.Count
        }).ToListAsync(_cancellationToken);
    }
}