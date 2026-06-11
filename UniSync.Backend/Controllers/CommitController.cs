using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSync.Backend.Data;
using UniSync.Shared.Models;

namespace UniSync.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommitController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommitController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Предложить правку (Доступно любому студенту)
    [HttpPost]
    public async Task<IActionResult> CreateCommit([FromBody] ScheduleCommit commit)
    {
        commit.Status = "Pending"; // Новая правка всегда в режиме ожидания
        _context.Commits.Add(commit);
        await _context.SaveChangesAsync();
        return Ok(commit);
    }

    // 2. Получить список всех правок (Для экрана старосты)
    [HttpGet]
    public async Task<IActionResult> GetCommits()
    {
        var commits = await _context.Commits.OrderByDescending(c => c.Id).ToListAsync();
        return Ok(commits);
    }

    // 3. Изменить статус правки (Одобрить/Отклонить — действие старосты)
    // Пример запроса: PUT /api/commit/5/status?status=Approved
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        if (status != "Approved" && status != "Rejected")
            return BadRequest("Допустимые статусы: Approved или Rejected");

        var commit = await _context.Commits.FindAsync(id);
        if (commit == null) return NotFound("Правка не найдена");

        commit.Status = status;
        await _context.SaveChangesAsync();
        return Ok(commit);
    }
}