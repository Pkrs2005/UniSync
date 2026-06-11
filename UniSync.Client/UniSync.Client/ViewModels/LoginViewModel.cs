using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniSync.Client.Services;

namespace UniSync.Client.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly Action _navigateToRegister;
    private readonly Action _navigateToMainApp;
    private readonly AuthService _authService = new();

    public LoginViewModel(Action navigateToRegister, Action navigateToMainApp)
    {
        _navigateToRegister = navigateToRegister;
        _navigateToMainApp = navigateToMainApp;
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task LoginAsync()
    {
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Введите логин и пароль!";
            return;
        }

        StatusMessage = "Проверка данных...";

        var success = await _authService.LoginAsync(Username, Password);
        if (success)
        {
            _navigateToMainApp();
            return;
        }

        StatusMessage = "Неверный логин или пароль";
    }

    [RelayCommand]
    private void Register()
    {
        _navigateToRegister();
    }
}
