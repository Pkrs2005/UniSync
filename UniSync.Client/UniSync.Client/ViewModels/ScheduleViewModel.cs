using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniSync.Client.Services;
using UniSync.Shared.Models;

namespace UniSync.Client.ViewModels;

public class DayScheduleGroup
{
    public DateTime Date { get; set; }

    public string DayHeader => CultureInfo.GetCultureInfo("ru-RU").TextInfo
        .ToTitleCase(Date.ToString("dddd, d MMMM", CultureInfo.GetCultureInfo("ru-RU")));

    public List<LessonDto> Lessons { get; set; } = new();
}

public partial class ScheduleViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly Action _navigateToCommits;
    private readonly Action _navigateToLogin;

    public ScheduleViewModel(Action navigateToCommits, Action navigateToLogin)
    {
        _navigateToCommits = navigateToCommits;
        _navigateToLogin = navigateToLogin;
        _apiService = new ApiService();
        _currentDate = DateTime.Today;
        IsModerator = AuthService.UserRole == "Moderator";
        _ = LoadScheduleAsync();
    }

    [ObservableProperty]
    private DateTime _currentDate;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _weekRangeText = "Загрузка...";

    [ObservableProperty]
    private bool _isModerator;

    [ObservableProperty]
    private LessonDto? _selectedLesson;

    [ObservableProperty]
    private bool _isCommitPanelVisible;

    [ObservableProperty]
    private string _newTitle = string.Empty;

    [ObservableProperty]
    private string _newRoom = string.Empty;

    [ObservableProperty]
    private string _newTeacher = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<DayScheduleGroup> GroupedLessons { get; } = new();

    [RelayCommand]
    private async Task MoveNextWeek()
    {
        CurrentDate = CurrentDate.AddDays(7);
        await LoadScheduleAsync();
    }

    [RelayCommand]
    private async Task MovePreviousWeek()
    {
        CurrentDate = CurrentDate.AddDays(-7);
        await LoadScheduleAsync();
    }

    [RelayCommand]
    private void SelectLesson(LessonDto lesson)
    {
        SelectedLesson = lesson;
        NewTitle = lesson.Title;
        NewRoom = lesson.Room ?? string.Empty;
        NewTeacher = lesson.Teacher ?? string.Empty;
        IsCommitPanelVisible = true;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private void CancelCommit()
    {
        IsCommitPanelVisible = false;
        SelectedLesson = null;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SubmitCommitAsync()
    {
        if (SelectedLesson == null) return;

        StatusMessage = "Отправка правки...";

        var commit = new ScheduleCommit
        {
            LessonDate = DateTime.SpecifyKind(SelectedLesson.Date.Date, DateTimeKind.Utc),
            StartTime = SelectedLesson.StartTime,
            OldTitle = SelectedLesson.Title,
            NewTitle = string.IsNullOrWhiteSpace(NewTitle) ? null : NewTitle.Trim(),
            NewRoom = string.IsNullOrWhiteSpace(NewRoom) ? null : NewRoom.Trim(),
            NewTeacher = string.IsNullOrWhiteSpace(NewTeacher) ? null : NewTeacher.Trim()
        };

        var success = await _apiService.CreateCommitAsync(commit);
        if (success)
        {
            StatusMessage = "Правка отправлена на модерацию!";
            IsCommitPanelVisible = false;
            SelectedLesson = null;
            return;
        }

        StatusMessage = "Ошибка отправки. Проверьте, что сервер запущен.";
    }

    [RelayCommand]
    private void OpenModeration()
    {
        _navigateToCommits();
    }

    [RelayCommand]
    private void Logout()
    {
        AuthService.ClearSession();
        _navigateToLogin();
    }

    public async Task LoadScheduleAsync()
    {
        IsLoading = true;
        var response = await _apiService.GetScheduleAsync(CurrentDate);

        GroupedLessons.Clear();

        if (response != null)
        {
            WeekRangeText = $"{response.StartOfWeek:dd.MM} — {response.EndOfWeek:dd.MM}";

            var sortedGroups = response.Lessons
                .GroupBy(l => l.Date.Date)
                .Select(g => new DayScheduleGroup
                {
                    Date = g.Key,
                    Lessons = g.OrderBy(l => l.StartTime).ToList()
                })
                .OrderBy(g => g.Date);

            foreach (var group in sortedGroups)
            {
                GroupedLessons.Add(group);
            }
        }
        else
        {
            WeekRangeText = "Ошибка загрузки";
        }

        IsLoading = false;
    }
}
