using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using UniSync.Client.ViewModels;
using UniSync.Client.Views;

namespace UniSync.Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("[LOG] App: OnFrameworkInitializationCompleted запущен.");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("[LOG] App: Обнаружен десктопный режим (IClassicDesktopStyleApplicationLifetime).");
            
            var mainVM = new MainViewModel();
            Console.WriteLine($"[LOG] App: Создан DataContext для окна. Проверка свойства CurrentPage: {mainVM.CurrentPage?.GetType().Name ?? "null"}");

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainVM
            };
            Console.WriteLine("[LOG] App: Главное окно MainWindow успешно создано и ему присвоен DataContext.");
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            Console.WriteLine("[LOG] App: Обнаружен SingleView режим (мобилки/веб).");
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
        Console.WriteLine("[LOG] App: Базовый метод OnFrameworkInitializationCompleted выполнен.");
    }
}