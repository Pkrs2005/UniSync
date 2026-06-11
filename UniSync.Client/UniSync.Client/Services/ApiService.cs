using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UniSync.Shared.Models;

namespace UniSync.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5134/api/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        ApplyAuthHeader();
    }

    private void ApplyAuthHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrEmpty(AuthService.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AuthService.Token);
        }
    }

    public async Task<ScheduleResponse?> GetScheduleAsync(DateTime? date = null)
    {
        ApplyAuthHeader();
        string url = date.HasValue
            ? $"schedule?date={date.Value:yyyy-MM-dd}"
            : "schedule";

        try
        {
            return await _httpClient.GetFromJsonAsync<ScheduleResponse>(url, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении расписания: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CreateCommitAsync(ScheduleCommit commit)
    {
        ApplyAuthHeader();
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

    public async Task<List<ScheduleCommit>> GetCommitsAsync()
    {
        ApplyAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ScheduleCommit>>("commit", JsonOptions) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении коммитов: {ex.Message}");
            return new List<ScheduleCommit>();
        }
    }

    public async Task<bool> UpdateCommitStatusAsync(int commitId, string status)
    {
        ApplyAuthHeader();
        try
        {
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
