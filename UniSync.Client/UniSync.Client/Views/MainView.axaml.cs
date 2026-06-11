using Avalonia.Controls;
using Avalonia.Markup.Xaml; // 👈 Обязательно добавь этот юзинг
using UniSync.Client.ViewModels;

namespace UniSync.Client.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new ScheduleViewModel();
    }

    // 👈 Пишем метод руками, раз генератор споткнулся о пробелы в путях
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}