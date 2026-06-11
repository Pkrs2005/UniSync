using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniSync.Client.Services;

namespace UniSync.Client.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly Action _navigateToLogin;
    private readonly AuthService _authService = new();

    public RegisterViewModel(Action navigateToLogin)
    {
        _navigateToLogin = navigateToLogin;
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Заполните все поля!";
            return;
        }

        if (Password.Length < 4)
        {
            StatusMessage = "Пароль должен быть не короче 4 символов";
            return;
        }

        StatusMessage = "Создание аккаунта...";

        var success = await _authService.RegisterAsync(Username, Password);
        if (success)
        {
            StatusMessage = "Аккаунт создан! Войдите в систему.";
            await Task.Delay(800);
            _navigateToLogin();
            return;
        }

        StatusMessage = "Не удалось зарегистрироваться (возможно, логин занят)";
    }

    [RelayCommand]
    private void BackToLogin()
    {
        _navigateToLogin();
    }
}
