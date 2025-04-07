using ClickCounter.Application.Services.Counter.DTOs;
using ClickCounter.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClickCounter.Application.Services.Counter;

public interface ICounterService {
    Task<List<CounterDto>> GetAllAsync();
    Task<CounterDto?> GetByIdAsync(int counterId);
    Task<CounterDto> AddAsync(SaveCounterDto saveCounterDto);
    Task<int> UpdateAsync(int counterId, SaveCounterDto saveCounterDto);
    Task<int> DeleteAsync(int counterId);
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

    public async Task<CounterDto?> GetByIdAsync(int counterId) {
        await using ClickCounterDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(_cancellationToken);
        
        return await dbContext.Counters.Where(counter => counter.CounterId == counterId && counter.Trash == 0).Select(counter => new CounterDto {
            CounterId = counter.CounterId,
            Name = counter.Name ?? string.Empty,
            Count = counter.Count
        }).FirstOrDefaultAsync(_cancellationToken);
    }

    public async Task<CounterDto> AddAsync(SaveCounterDto saveCounterDto) {
        await using ClickCounterDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(_cancellationToken);

        Domain.Entities.Counter counter = new() {
            Name = saveCounterDto.Name,
            Count = saveCounterDto.Count,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Trash = 0
        };
        await dbContext.Counters.AddAsync(counter, _cancellationToken);
        await dbContext.SaveChangesAsync(_cancellationToken);
        
        return new CounterDto {
            CounterId = counter.CounterId,
            Name = counter.Name ?? string.Empty,
            Count = counter.Count
        };
    }

    public async Task<int> UpdateAsync(int counterId, SaveCounterDto saveCounterDto) {
        await using ClickCounterDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(_cancellationToken);
        
        return await dbContext.Counters.Where(counter => counter.CounterId == counterId && counter.Trash == 0)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(counter => counter.Name, saveCounterDto.Name)
                .SetProperty(counter => counter.Count, saveCounterDto.Count)
                .SetProperty(counter => counter.UpdatedAt, DateTime.Now), _cancellationToken);
    }

    public async Task<int> DeleteAsync(int counterId) {
        await using ClickCounterDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(_cancellationToken);
        
        return await dbContext.Counters.Where(counter => counter.CounterId == counterId && counter.Trash == 0)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(counter => counter.Trash, 1)
                .SetProperty(counter => counter.UpdatedAt, DateTime.Now), _cancellationToken);
    }
}