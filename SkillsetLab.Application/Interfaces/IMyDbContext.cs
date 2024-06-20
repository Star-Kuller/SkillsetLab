namespace Application.Interfaces;

public interface IMyDbContext
{
    public List<Task> Task { get; set; }
}