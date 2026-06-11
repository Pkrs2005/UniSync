using System.Collections.Generic;

namespace UniSync.Shared.Models;

public class ScheduleResponse
{
    public DateTime StartOfWeek { get; set; }
    public DateTime EndOfWeek { get; set; }
    public List<LessonDto> Lessons { get; set; } = new();
}