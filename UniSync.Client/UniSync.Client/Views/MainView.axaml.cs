using System;
using Avalonia.Controls;

namespace UniSync.Client.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Console.WriteLine("[LOG] MainView: Метод InitializeComponent() выполнен.");

        this.DataContextChanged += (s, e) =>
        {
            Console.WriteLine($"[LOG] MainView: DataContext изменился! Текущий тип: {this.DataContext?.GetType().Name ?? "null"}");
        };
    }
}