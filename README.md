# 🌙 Yoru Engine

### Flexible 2D game/application framework for crossplatform development

## About

Yoru is a software development framework written in C#, designed for creating crossplatform apps and games. Backed by the capabilities of SkiaSharp, Yoru provides a direct approach towards developing scalable and production-ready software, that runs smoothly across [all platforms](#platforms). See the [example project](Yoru.Example/Program.cs) for a demonstration of Yoru's capabilities.

### Structure

Applications made with Yoru have a simple structure, consisting of `Application`, `Desktop` and `Mobile` projects.

- `Application` is the main project, where the application logic is written.
- `Desktop` is the project for desktop applications, and is used to run the application on Windows, macOS and Linux.
- `Mobile` is the project for mobile applications, and is used to run the application on iOS and Android.

Yoru provides an abstract `Renderer` class, that allows a single SkiaSharp canvas to be consistently used across all platforms. Audio, input, and other features undergo a similar process, allowing for a seamless development experience.

### Platforms

Yoru was designed with the goal of allowing a single C# project to run smoothly across common operating systems, without requiring platform-specific code. Yoru utilises both [OpenTK](https://opentk.net/) and [.NET Maui](https://dotnet.microsoft.com/en-us/apps/maui) to provide native support for Window, macOS, Linux, iOS and Android, and bridges platform-specific commands to a global interface for straightforward development.

The following platforms are (planned to be) supported by Yoru:

- [x] Windows 10+
- [x] macOS 10.12+
- [x] Linux (Ubuntu, Fedora, etc.)
- [ ] iOS 11+
- [ ] Android 5.0+

## Installation

#### You may find the Nuget package [Here.](https://www.nuget.org/packages/Yoru)

### .NET CLI

`dotnet add package Yoru --prerelease`

### Package Manager

`NuGet\Install-Package Yoru -Version 0.1.0`

## Usage

### More information and/or a wiki will be added later. For an example project, see [the example project.](Yoru.Example/Program.cs)
