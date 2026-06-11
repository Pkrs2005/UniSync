using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniSync.Client.Services;
using UniSync.Shared.Models;

namespace UniSync.Client.ViewModels;

public partial class CommitsViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly Action _navigateToSchedule;
    private readonly Action _navigateToLogin;

    public CommitsViewModel(Action navigateToSchedule, Action navigateToLogin)
    {
        _navigateToSchedule = navigateToSchedule;
        _navigateToLogin = navigateToLogin;
        _apiService = new ApiService();
        _ = LoadCommitsAsync();
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<ScheduleCommit> Commits { get; } = new();

    [RelayCommand]
    private async Task LoadCommitsAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        Commits.Clear();

        var commits = await _apiService.GetCommitsAsync();
        foreach (var commit in commits)
        {
            Commits.Add(commit);
        }

        if (commits.Count == 0)
            StatusMessage = "Нет предложенных правок";

        IsLoading = false;
    }

    [RelayCommand]
    private async Task ApproveAsync(ScheduleCommit commit)
    {
        await UpdateStatusAsync(commit, "Approved");
    }

    [RelayCommand]
    private async Task RejectAsync(ScheduleCommit commit)
    {
        await UpdateStatusAsync(commit, "Rejected");
    }

    private async Task UpdateStatusAsync(ScheduleCommit commit, string status)
    {
        var success = await _apiService.UpdateCommitStatusAsync(commit.Id, status);
        if (success)
        {
            commit.Status = status;
            OnPropertyChanged(nameof(Commits));
            await LoadCommitsAsync();
            return;
        }

        StatusMessage = "Не удалось обновить статус";
    }

    [RelayCommand]
    private void BackToSchedule()
    {
        _navigateToSchedule();
    }

    [RelayCommand]
    private void Logout()
    {
        AuthService.ClearSession();
        _navigateToLogin();
    }
}
