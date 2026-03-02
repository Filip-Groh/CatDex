using System.Windows.Input;

namespace CatDex.Controls;

public partial class CircularIconButton : ContentView
{
    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(CircularIconButton));

    public ImageSource IconSource
    {
        get => (ImageSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CircularIconButton));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CircularIconButton));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(double), typeof(CircularIconButton), 50.0,
            propertyChanged: OnSizeChanged);

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly BindableProperty IconPaddingProperty =
        BindableProperty.Create(nameof(IconPadding), typeof(Thickness), typeof(CircularIconButton), new Thickness(10));

    public Thickness IconPadding
    {
        get => (Thickness)GetValue(IconPaddingProperty);
        set => SetValue(IconPaddingProperty, value);
    }

    public static readonly BindableProperty IsButtonEnabledProperty =
        BindableProperty.Create(nameof(IsButtonEnabled), typeof(bool), typeof(CircularIconButton), true);

    public bool IsButtonEnabled
    {
        get => (bool)GetValue(IsButtonEnabledProperty);
        set => SetValue(IsButtonEnabledProperty, value);
    }

    public int CornerRadiusValue => (int)(Size / 2);

    private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularIconButton button)
        {
            button.OnPropertyChanged(nameof(CornerRadiusValue));
        }
    }

    public CircularIconButton()
    {
        InitializeComponent();
    }
}
