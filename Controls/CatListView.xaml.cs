namespace CatDex.Controls;

public partial class CatListView : ContentView
{
    public static readonly BindableProperty EmptyViewTextProperty =
        BindableProperty.Create(nameof(EmptyViewText), typeof(string), typeof(CatListView), "No cats found");

    public string EmptyViewText
    {
        get => (string)GetValue(EmptyViewTextProperty);
        set => SetValue(EmptyViewTextProperty, value);
    }

    public CatListView()
    {
        InitializeComponent();
    }
}
