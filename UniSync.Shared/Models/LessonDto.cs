namespace UniSync.Shared.Models;

public class LessonDto
{
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Room { get; set; }
    public string? Teacher { get; set; }
}