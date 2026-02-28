namespace CatDex.Services.Interfaces;

public interface IFileSaverService
{
	Task<bool> SaveImageAsync(string? url, byte[]? imageBytes, string fileName);
}
