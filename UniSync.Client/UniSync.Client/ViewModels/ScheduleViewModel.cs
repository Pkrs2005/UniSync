using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UniSync.Shared.Models;
using UniSync.Client.Services;

namespace UniSync.Client.ViewModels;

// 👇 Класс для группировки пар по конкретным дням
public class DayScheduleGroup
{
    public DateTime Date { get; set; }
    
    // Красивое форматирование заголовка: "Понедельник, 8 июня"
    public string DayHeader => CultureInfo.GetCultureInfo("ru-RU").TextInfo
        .ToTitleCase(Date.ToString("dddd, d MMMM", CultureInfo.GetCultureInfo("ru-RU")));

    public List<LessonDto> Lessons { get; set; } = new();
}

public class ScheduleViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;
    private DateTime _currentDate;
    private bool _isLoading;
    private string _weekRangeText = "Загрузка...";

    // 👇 Меняем плоский список Lessons на сгруппированную по дням коллекцию
    public ObservableCollection<DayScheduleGroup> GroupedLessons { get; } = new();

    public DateTime CurrentDate
    {
        get => _currentDate;
        set { _currentDate = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public string WeekRangeText
    {
        get => _weekRangeText;
        set { _weekRangeText = value; OnPropertyChanged(); }
    }

    public ScheduleViewModel()
    {
        // Предполагается, что ApiService принимает базовый URL или настроен внутри
        _apiService = new ApiService();
        _currentDate = DateTime.Today;
        
        // Триггерим первичную загрузку данных
        _ = LoadScheduleAsync();
    }

    // Основной метод загрузки данных с бэкенда
    public async Task LoadScheduleAsync()
    {
        IsLoading = true;
        var response = await _apiService.GetScheduleAsync(CurrentDate);
        
        // Очищаем старое расписание перед заполнением новой недели
        GroupedLessons.Clear();

        if (response != null)
        {
            WeekRangeText = $"{response.StartOfWeek:dd.MM} — {response.EndOfWeek:dd.MM}";
            
            // 👇 МАГИЯ LINQ: Группируем пары по дате дня, сортируем дни по хронологии, 
            // а пары внутри каждого дня — по времени начала.
            var sortedGroups = response.Lessons
                .GroupBy(l => l.Date.Date)
                .Select(g => new DayScheduleGroup
                {
                    Date = g.Key,
                    Lessons = g.OrderBy(l => l.StartTime).ToList()
                })
                .OrderBy(g => g.Date);

            // Наполняем ObservableCollection для автоматического обновления интерфейса
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

    // Методы для кнопок "Вперед" и "Назад" в UI
    public async Task MoveNextWeek()
    {
        CurrentDate = CurrentDate.AddDays(7);
        await LoadScheduleAsync();
    }

    public async Task MovePreviousWeek()
    {
        CurrentDate = CurrentDate.AddDays(-7);
        await LoadScheduleAsync();
    }

    // Реализация интерфейса для обновления UI при изменении свойств
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}