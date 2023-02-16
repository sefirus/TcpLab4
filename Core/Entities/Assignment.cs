namespace Core.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public string AssigneeName { get; set; }
    public HashSet<Question> Questions { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}