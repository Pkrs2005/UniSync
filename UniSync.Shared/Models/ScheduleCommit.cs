namespace UniSync.Shared.Models;

public class ScheduleCommit
{
    public int Id { get; set; }
    
    // Поля для точной идентификации оригинальной пары, которую правим
    public DateTime LessonDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string OldTitle { get; set; } = string.Empty; 

    // Новые (предлагаемые) данные
    public string? NewTitle { get; set; }
    public string? NewRoom { get; set; }
    public string? NewTeacher { get; set; }

    // Статус правки: "Pending" (ожидает), "Approved" (одобрено), "Rejected" (отклонено)
    public string Status { get; set; } = "Pending";

    public bool IsPending => Status == "Pending";
}