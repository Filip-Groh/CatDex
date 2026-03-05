using System.Windows.Input;
using CatDex.Models;

namespace CatDex.Controls;

public partial class CatCard : ContentView
{
    private ICommand? _originalToggleFavoriteCommand;

    public static readonly BindableProperty ToggleFavoriteCommandProperty =
        BindableProperty.Create(
            nameof(ToggleFavoriteCommand), 
            typeof(ICommand), 
            typeof(CatCard), 
            propertyChanged: OnToggleFavoriteCommandChanged);

    public ICommand ToggleFavoriteCommand
    {
        get => (ICommand)GetValue(ToggleFavoriteCommandProperty);
        set => SetValue(ToggleFavoriteCommandProperty, value);
    }

    private static void OnToggleFavoriteCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CatCard catCard && newValue is ICommand command)
        {
            catCard._originalToggleFavoriteCommand = command;
        }
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

    public static readonly BindableProperty IsFavoriteProperty =
        BindableProperty.Create(nameof(IsFavorite), typeof(bool), typeof(CatCard), false, BindingMode.TwoWay, propertyChanged: OnIsFavoriteChanged);

    public bool IsFavorite
    {
        get => (bool)GetValue(IsFavoriteProperty);
        set => SetValue(IsFavoriteProperty, value);
    }

    private static void OnIsFavoriteChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CatCard catCard && newValue is bool isFavorite)
        {
            catCard.FavoriteIcon = isFavorite ? "favorite_full.svg" : "favorite_empty.svg";
        }
    }

    public static readonly BindableProperty FavoriteIconProperty =
        BindableProperty.Create(nameof(FavoriteIcon), typeof(string), typeof(CatCard), "favorite_empty.svg");

    public string FavoriteIcon
    {
        get => (string)GetValue(FavoriteIconProperty);
        set => SetValue(FavoriteIconProperty, value);
    }

    public CatCard()
    {
        InitializeComponent();
        BindingContextChanged += OnBindingContextChanged;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is Cat cat)
        {
            IsFavorite = cat.IsFavorite;
        }
    }

    private void ExecuteToggleFavorite(Cat cat)
    {
        if (cat == null)
            return;

        IsFavorite = !IsFavorite;

        if (_originalToggleFavoriteCommand?.CanExecute(cat) == true)
        {
            _originalToggleFavoriteCommand.Execute(cat);
        }
    }

    private void FavoriteButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Cat cat)
        {
            ExecuteToggleFavorite(cat);
        }
    }
}