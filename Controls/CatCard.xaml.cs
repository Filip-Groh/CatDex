using System.Windows.Input;

namespace CatDex.Controls;

public partial class CatCard : ContentView
{
    public static readonly BindableProperty ToggleFavoriteCommandProperty =
        BindableProperty.Create(nameof(ToggleFavoriteCommand), typeof(ICommand), typeof(CatCard));

    public ICommand ToggleFavoriteCommand
    {
        get => (ICommand)GetValue(ToggleFavoriteCommandProperty);
        set => SetValue(ToggleFavoriteCommandProperty, value);
    }

    public static readonly BindableProperty TappedCommandProperty =
        BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(CatCard));

    public ICommand TappedCommand
    {
        get => (ICommand)GetValue(TappedCommandProperty);
        set => SetValue(TappedCommandProperty, value);
    }

    public static readonly BindableProperty IsListModeProperty =
        BindableProperty.Create(nameof(IsListMode), typeof(bool), typeof(CatCard), false);

    public bool IsListMode
    {
        get => (bool)GetValue(IsListModeProperty);
        set => SetValue(IsListModeProperty, value);
    }

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(CatCard));

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public CatCard()
    {
        InitializeComponent();
    }
}