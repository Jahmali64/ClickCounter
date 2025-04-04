namespace ClickCounter.Domain.Entities;

public partial class Counter {
    public int CounterId { get; set; }

    public string? Name { get; set; }

    public int Count { get; set; }

    public int Trash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
