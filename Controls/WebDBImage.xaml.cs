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

	public static readonly BindableProperty EnableFullScreenOnTapProperty = BindableProperty.Create(
		nameof(EnableFullScreenOnTap),
		typeof(bool),
		typeof(WebDBImage),
		true,
		propertyChanged: OnEnableFullScreenOnTapChanged);

	private TapGestureRecognizer? _tapGestureRecognizer;

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

	public bool EnableFullScreenOnTap
	{
		get => (bool)GetValue(EnableFullScreenOnTapProperty);
		set => SetValue(EnableFullScreenOnTapProperty, value);
	}

	public WebDBImage()
	{
		InitializeComponent();
		UpdateImageSource();
	}

	protected override void OnParentSet()
	{
		base.OnParentSet();
		if (Parent != null)
		{
			UpdateGestureRecognizer();
		}
	}

	private static void OnEnableFullScreenOnTapChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is WebDBImage webDBImage)
		{
			webDBImage.UpdateGestureRecognizer();
		}
	}

	private void UpdateGestureRecognizer()
	{
		if (DisplayImage == null)
			return;

		// Remove existing gesture recognizer if any
		if (_tapGestureRecognizer != null)
		{
			DisplayImage.GestureRecognizers.Remove(_tapGestureRecognizer);
			_tapGestureRecognizer.Tapped -= OnImageTapped;
			_tapGestureRecognizer = null;
		}

		// Add gesture recognizer only if enabled
		if (EnableFullScreenOnTap)
		{
			_tapGestureRecognizer = new TapGestureRecognizer();
			_tapGestureRecognizer.Tapped += OnImageTapped;
			DisplayImage.GestureRecognizers.Add(_tapGestureRecognizer);
		}
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

	private async void OnImageTapped(object? sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>();

		if (!string.IsNullOrEmpty(Url))
		{
			navigationParameter["ImageUrl"] = Url;
		}

		if (ImageData != null)
		{
			navigationParameter["ImageData"] = ImageData;
		}

		if (navigationParameter.Count > 0)
		{
			await Shell.Current.GoToAsync(nameof(Views.FullScreenImagePage), navigationParameter);
		}
	}
}
