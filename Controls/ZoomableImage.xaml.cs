namespace CatDex.Controls;

public partial class ZoomableImage : ContentView
{
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(ZoomableImage),
        default(ImageSource),
        propertyChanged: OnSourceChanged);

    private double _currentScale = 1;
    private const double MaxScale = 4;
    private const double MinScale = 1;

    public event EventHandler<bool>? ZoomStateChanged;

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public bool IsZoomed => _currentScale > MinScale;

    public ZoomableImage()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        // Apply scale whenever container size changes to keep image properly sized
        ApplyScale();
    }

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableImage zoomableImage && newValue is ImageSource source)
        {
            zoomableImage.ZoomImage.Source = source;
        }
    }

    private void OnZoomChanged(object? sender, ValueChangedEventArgs e)
    {
        var wasZoomed = IsZoomed;
        _currentScale = e.NewValue;
        ApplyScale();

        // Notify if zoom state changed
        if (wasZoomed != IsZoomed)
        {
            ZoomStateChanged?.Invoke(this, IsZoomed);
        }
    }

    private void OnZoomIn(object? sender, EventArgs e)
    {
        _currentScale = Math.Min(_currentScale + 0.5, MaxScale);
        ApplyScale();
        ZoomStateChanged?.Invoke(this, IsZoomed);
    }

    private void OnZoomOut(object? sender, EventArgs e)
    {
        var wasZoomed = IsZoomed;
        _currentScale = Math.Max(_currentScale - 0.5, MinScale);
        ApplyScale();

        if (wasZoomed != IsZoomed)
        {
            ZoomStateChanged?.Invoke(this, IsZoomed);
        }
    }

    private void ApplyScale()
    {
        // Skip if container hasn't been sized yet
        if (Width <= 0 || Height <= 0)
            return;

        // Use WidthRequest/HeightRequest for zooming (not Scale) so ScrollView can scroll to edges
        ZoomImage.WidthRequest = Width * _currentScale;
        ZoomImage.HeightRequest = Height * _currentScale;
    }
}
