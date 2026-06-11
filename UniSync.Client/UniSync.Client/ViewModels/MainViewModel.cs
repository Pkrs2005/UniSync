using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UniSync.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentPage;

    public MainViewModel()
    {
        Console.WriteLine("[LOG] Инициализация MainViewModel началась.");
        NavigateToLogin();
    }

    public void NavigateToLogin()
    {
        CurrentPage = new LoginViewModel(
            navigateToRegister: NavigateToRegister,
            navigateToMainApp: NavigateToSchedule
        );
        Console.WriteLine("[LOG] Экран изменен на: LoginViewModel");
    }

    public void NavigateToRegister()
    {
        CurrentPage = new RegisterViewModel(
            navigateToLogin: NavigateToLogin
        );
        Console.WriteLine("[LOG] Экран изменен на: RegisterViewModel");
    }

    public void NavigateToSchedule()
    {
        CurrentPage = new ScheduleViewModel(
            navigateToCommits: NavigateToCommits,
            navigateToLogin: NavigateToLogin
        );
        Console.WriteLine("[LOG] Экран изменен на: ScheduleViewModel");
    }

    public void NavigateToCommits()
    {
        CurrentPage = new CommitsViewModel(
            navigateToSchedule: NavigateToSchedule,
            navigateToLogin: NavigateToLogin
        );
        Console.WriteLine("[LOG] Экран изменен на: CommitsViewModel");
    }
}