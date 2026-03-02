# CatDex

CatDex is a cross-platform mobile app built with .NET MAUI that lets you discover, save, and manage information about cats and cat breeds.

## Features

- **Discover** – Browse random cat images fetched from [The Cat API](https://thecatapi.com), complete with breed information.
- **Favorites** – Mark cats as favorites and revisit them at any time.
- **Seen** – Keep track of every cat you have already viewed.
- **Create** – Add your own custom cat entries to your personal collection.
- **Cat Details** – View in-depth information for each cat, including breed description and traits.
- **Full-screen Images** – Open any cat image in a zoomable full-screen viewer.
- **Settings** – Customize app preferences such as the color theme.

## Technologies

| Technology | Purpose |
|---|---|
| [.NET MAUI](https://learn.microsoft.com/dotnet/maui/) | Cross-platform UI framework (Android, iOS, macOS, Windows) |
| C# / XAML | Application language and UI markup |
| [Entity Framework Core + SQLite](https://learn.microsoft.com/ef/core/) | Local database for storing cats |
| [The Cat API](https://thecatapi.com) | Remote source for cat images and breed data |

## Getting Started

1. Clone the repository.
2. Open `CatDex.slnx` in Visual Studio 2022 (or later) with the .NET MAUI workload installed.
3. Select your target platform (Android, iOS, macOS, or Windows) and run the project.
