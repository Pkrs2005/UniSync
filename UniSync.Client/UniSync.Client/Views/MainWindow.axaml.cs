using System;
using Avalonia.Controls;

namespace UniSync.Client.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Console.WriteLine("[LOG] MainWindow: Метод InitializeComponent() выполнен.");

        // Логируем изменение DataContext
        this.DataContextChanged += (s, e) =>
        {
            Console.WriteLine($"[LOG] MainWindow: DataContext изменился! Текущий тип: {this.DataContext?.GetType().Name ?? "null"}");
        };
    }
}