namespace ClickCounter.Application.Services.Counter.DTOs;

public sealed class SaveCounterDto {
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}