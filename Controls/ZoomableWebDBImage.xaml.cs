using CatDex.Models;

namespace CatDex.Controls;

public partial class ZoomableWebDBImage : ContentView
{
    public static readonly BindableProperty UrlProperty = BindableProperty.Create(
        nameof(Url),
        typeof(string),
        typeof(ZoomableWebDBImage),
        default(string),
        propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty ImageDataProperty = BindableProperty.Create(
        nameof(ImageData),
        typeof(ImageData),
        typeof(ZoomableWebDBImage),
        default(ImageData),
        propertyChanged: OnImageSourceChanged);

    private double _currentScale = 1;
    private const double MaxScale = 4;
    private const double MinScale = 1;

    public event EventHandler<bool>? ZoomStateChanged;

    public string? Url
    {
        get => (string?)GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    public ImageData? ImageData
    {
        get => (ImageData?)GetValue(ImageDataProperty);
        set => SetValue(ImageDataProperty, value);
    }

    public bool IsZoomed => _currentScale > MinScale;

    public ZoomableWebDBImage()
    {
        InitializeComponent();
        UpdateImageSource();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        // Apply scale whenever container size changes to keep image properly sized
        ApplyScale();
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ZoomableWebDBImage webDBImage)
        {
            webDBImage.UpdateImageSource();
        }
    }

    private void UpdateImageSource()
    {
        if (DisplayImage == null)
            return;

        if (ImageData?.Bytes != null && ImageData.Bytes.Length > 0)
        {
            var bytes = ImageData.Bytes;
            DisplayImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        else if (!string.IsNullOrEmpty(Url))
        {
            DisplayImage.Source = Url;
        }
        else
        {
            DisplayImage.Source = null;
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

    private void ApplyScale()
    {
        // Skip if container hasn't been sized yet
        if (Width <= 0 || Height <= 0)
            return;

        // Use WidthRequest/HeightRequest for zooming (not Scale) so ScrollView can scroll to edges
        DisplayImage.WidthRequest = Width * _currentScale;
        DisplayImage.HeightRequest = Height * _currentScale;
    }
}
