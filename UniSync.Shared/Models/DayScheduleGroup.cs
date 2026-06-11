using System;
using System.Collections.Generic;
using System.Globalization;
using UniSync.Shared.Models;

namespace UniSync.Client.Models; // Или используй свой namespace

public class DayScheduleGroup
{
    public DateTime Date { get; set; }
    
    // Красивое название дня, например: "Понедельник, 08 июня"
    public string DayHeader => CultureInfo.GetCultureInfo("ru-RU").TextInfo
        .ToTitleCase(Date.ToString("dddd, d MMMM", CultureInfo.GetCultureInfo("ru-RU")));

    public List<LessonDto> Lessons { get; set; } = new();
}