using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UniSync.Shared.Models;

namespace UniSync.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    // Адрес твоего запущенного бэкенда
    private const string BaseUrl = "http://localhost:5134/api/"; 

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // 1. Получить расписание на определенную дату (или текущую неделю)
    public async Task<ScheduleResponse?> GetScheduleAsync(DateTime? date = null)
    {
        string url = date.HasValue 
            ? $"schedule?date={date.Value:yyyy-MM-dd}" 
            : "schedule";

        try
        {
            return await _httpClient.GetFromJsonAsync<ScheduleResponse>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении расписания: {ex.Message}");
            return null;
        }
    }

    // 2. Отправить новый коммит (предложение правки) от студента
    public async Task<bool> CreateCommitAsync(ScheduleCommit commit)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("commit", commit);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке коммита: {ex.Message}");
            return false;
        }
    }

    // 3. Получить список всех коммитов (для экрана старосты)
    public async Task<List<ScheduleCommit>> GetCommitsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ScheduleCommit>>("commit") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении коммитов: {ex.Message}");
            return new List<ScheduleCommit>();
        }
    }

    // 4. Одобрить или отклонить коммит (действие старосты)
    public async Task<bool> UpdateCommitStatusAsync(int commitId, string status)
    {
        try
        {
            // PUT /api/commit/{id}/status?status=Approved
            var response = await _httpClient.PutAsync($"commit/{commitId}/status?status={status}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении статуса коммита: {ex.Message}");
            return false;
        }
    }
}