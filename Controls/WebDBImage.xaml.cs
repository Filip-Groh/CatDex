using CatDex.Models;

namespace CatDex.Controls;

public partial class WebDBImage : ContentView
{
	public static readonly BindableProperty UrlProperty = BindableProperty.Create(
		nameof(Url),
		typeof(string),
		typeof(WebDBImage),
		default(string),
		propertyChanged: OnImageSourceChanged);

	public static readonly BindableProperty ImageDataProperty = BindableProperty.Create(
		nameof(ImageData),
		typeof(ImageData),
		typeof(WebDBImage),
		default(ImageData),
		propertyChanged: OnImageSourceChanged);

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

	public WebDBImage()
	{
		InitializeComponent();
		UpdateImageSource();
	}

	private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is WebDBImage webDBImage)
		{
			webDBImage.UpdateImageSource();
		}
	}

	private void UpdateImageSource()
	{
		if (ImageData?.Bytes != null && ImageData.Bytes.Length > 0)
		{
			DisplayImage.Source = ImageSource.FromStream(() => new MemoryStream(ImageData.Bytes));
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
}