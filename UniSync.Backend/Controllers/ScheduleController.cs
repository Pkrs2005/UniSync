using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ical.Net;
using UniSync.Backend.Data;
using UniSync.Shared.Models;

namespace UniSync.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;

    public ScheduleController(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedule([FromQuery] DateTime? date)
    {
        DateTime targetDate = date ?? DateTime.Today;
        int diff = (7 + (targetDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = targetDate.AddDays(-1 * diff).Date;
        DateTime endOfWeek = startOfWeek.AddDays(6);

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:151.0) Gecko/20100101 Firefox/151.0");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/calendar");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://rdcenter.ru");

        try
        {
            string url = "https://schedule.rdcenter.ru/api/Schedule/ics?attendeePersonId=1cca45c9-84ce-4d04-94df-7c26d87542a9";
            string icsText = await client.GetStringAsync(url);
            var calendar = Calendar.Load(icsText);

            var events = calendar.Events ?? Enumerable.Empty<Ical.Net.CalendarComponents.CalendarEvent>();
            var lessons = events
                .Where(e => e.Start != null && 
                            e.Start.AsUtc.ToLocalTime().Date >= startOfWeek && 
                            e.Start.AsUtc.ToLocalTime().Date <= endOfWeek)
                .Select(ev => 
                {
                    var startTime = ev.Start!.AsUtc.ToLocalTime();
                    
                    // Защита от NullReferenceException: если End нет, берем Start + 90 минут
                    var endTime = ev.End != null ? ev.End.AsUtc.ToLocalTime() : startTime.AddMinutes(90);
                    
                    string rawSummary = ev.Summary ?? string.Empty;
                    string cleanSummary = rawSummary.Contains('/') ? rawSummary.Split('/').First().Trim() : rawSummary;

                    string description = ev.Description ?? "";
                    string? room = null;
                    string? teacher = null;

                    if (!string.IsNullOrEmpty(description))
                    {
                        var lines = description.Split(new[] { "\\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        var roomLine = lines.FirstOrDefault(l => l.Trim().StartsWith("Аудитория:"));
                        var teacherLine = lines.FirstOrDefault(l => l.Trim().StartsWith("Преподаватели:"));

                        if (roomLine != null) room = roomLine.Replace("Аудитория:", "").Trim();
                        if (teacherLine != null) teacher = teacherLine.Replace("Преподаватели:", "").Replace("Препод:", "").Trim();
                    }

                    return new LessonDto
                    {
                        Date = startTime.Date,
                        StartTime = startTime.ToString("HH:mm"),
                        EndTime = endTime.ToString("HH:mm"),
                        Title = cleanSummary,
                        Room = room,
                        Teacher = teacher
                    };
                })
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .ToList();

            // ИСПРАВЛЕНИЕ: Размечаем как UTC, так как Postgres требует именно этот Kind для timestamp with time zone
            DateTime startOfWeekUtc = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
            DateTime endOfWeekUtc = DateTime.SpecifyKind(endOfWeek, DateTimeKind.Utc);

            // Загружаем одобренные коммиты из БД
            var approvedCommits = await _context.Commits
                .Where(c => c.Status == "Approved" && c.LessonDate >= startOfWeekUtc && c.LessonDate <= endOfWeekUtc)
                .ToListAsync();

            // Перебираем расписание и подменяем значения
            foreach (var lesson in lessons)
            {
                // Безопасный string.Equals на случай null в БД
                var match = approvedCommits.FirstOrDefault(c => 
                    c.LessonDate.Date == lesson.Date.Date && 
                    c.StartTime == lesson.StartTime && 
                    string.Equals(c.OldTitle, lesson.Title, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    if (!string.IsNullOrEmpty(match.NewTitle)) lesson.Title = match.NewTitle;
                    if (!string.IsNullOrEmpty(match.NewRoom)) lesson.Room = match.NewRoom;
                    if (!string.IsNullOrEmpty(match.NewTeacher)) lesson.Teacher = match.NewTeacher;
                }
            }

            return Ok(new { StartOfWeek = startOfWeek, EndOfWeek = endOfWeek, Lessons = lessons });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ошибка при обработке расписания бэкендом", Details = ex.ToString() });
        }
    }
}