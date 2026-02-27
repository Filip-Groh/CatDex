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

    public CatCard()
    {
        InitializeComponent();
    }
}