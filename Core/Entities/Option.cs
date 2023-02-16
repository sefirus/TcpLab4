namespace Core.Entities;

public class Option
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public bool IsCorrect { get; set; }
}