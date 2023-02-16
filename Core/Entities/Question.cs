namespace Core.Entities;    

public class Question
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public HashSet<Option> Options { get; set; } = new();
    public Guid CorrectAnswerId { get; set; }
    public Option CorrectAnswer { get; set; }
    public Guid? ChosenAnswerId { get; set; }
    public Option? ChosenAnswer { get; set; }
}