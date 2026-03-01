using CatDex.Services.Interfaces;
using CatDex.Models.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CatDex.Models;

namespace CatDex.ViewModels
{
    public partial class CreateViewModel : ObservableObject
    {
        private readonly ICatRepositoryService _repository;

        [ObservableProperty]
        public partial string CatId { get; set; } = string.Empty;

        [ObservableProperty]
        public partial int Width { get; set; } = 500;

        [ObservableProperty]
        public partial int Height { get; set; } = 500;

        [ObservableProperty]
        public partial byte[]? ImageBytes { get; set; }

        [ObservableProperty]
        public partial string? ImageSource { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string? ErrorMessage { get; set; }

        [ObservableProperty]
        public partial string? SuccessMessage { get; set; }

        public ObservableCollection<Breed> AvailableBreeds { get; } = new();
        public ObservableCollection<Breed> SelectedBreeds { get; } = new();

        public CreateViewModel(ICatRepositoryService repository)
        {
            _repository = repository;
            Task.Run(async () => await LoadBreeds());
        }

        private async Task LoadBreeds()
        {
            try
            {
                var breeds = await _repository.GetBreedsAsync();
                foreach (var breed in breeds)
                {
                    AvailableBreeds.Add(breed);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load breeds: {ex.Message}";
            }
        }

        [RelayCommand]
        async Task PickImage()
        {
            try
            {
                ErrorMessage = null;
                var results = await MediaPicker.Default.PickPhotosAsync();

                if (results != null && results.Count > 0)
                {
                    var result = results[0];
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    ImageBytes = memoryStream.ToArray();
                    ImageSource = result.FullPath;

                    await ExtractImageDimensions(ImageBytes);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error picking image: {ex.Message}";
            }
        }

        private async Task ExtractImageDimensions(byte[] imageBytes)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var ms = new MemoryStream(imageBytes);
                    var imageInfo = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(ms);
                    if (imageInfo != null)
                    {
                        Width = (int)imageInfo.Width;
                        Height = (int)imageInfo.Height;
                    }
                }
                catch
                {
                    Width = 500;
                    Height = 500;
                }
            });
        }

        [RelayCommand]
        async Task CreateCat()
        {
            try
            {
                ErrorMessage = null;
                SuccessMessage = null;

                if (ImageBytes == null || ImageBytes.Length == 0)
                {
                    ErrorMessage = "Please select an image";
                    return;
                }

                IsBusy = true;

                var generatedId = $"custom_{Guid.NewGuid():N}";

                var customCat = new CustomCatDTO
                {
                    Id = generatedId,
                    Width = Width,
                    Height = Height,
                    Bytes = ImageBytes,
                    BreedIds = SelectedBreeds.Select(b => b.Id).ToList()
                };

                var createdCat = await _repository.CreateCatAsync(customCat);
                SuccessMessage = $"Cat '{createdCat.Id}' created successfully!";

                CatId = string.Empty;
                Width = 500;
                Height = 500;
                ImageBytes = null;
                ImageSource = null;
                SelectedBreeds.Clear();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating cat: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        void ToggleBreed(Breed breed)
        {
            if (SelectedBreeds.Contains(breed))
            {
                SelectedBreeds.Remove(breed);
            }
            else
            {
                SelectedBreeds.Add(breed);
            }
            OnPropertyChanged(nameof(SelectedBreeds));
        }

        public bool IsBreedSelected(Breed breed)
        {
            return SelectedBreeds.Contains(breed);
        }
    }
}
