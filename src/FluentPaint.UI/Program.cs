using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace FluentPaint.UI;

/// <summary>
/// The entry point for application startup.
/// </summary>
class Program
{
    /// <summary>
    /// Initialization code.
    /// </summary>
    /// <remarks>
    /// Don't use any Avalonia, third-party APIs or any SynchronizationContext-reliant code
    /// before AppMain is called: things aren't initialized yet and stuff might break.
    /// </remarks>
    /// <param name="args"> Program arguments</param>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Avalonia configuration
    /// </summary>
    /// <remarks>
    /// Also used by visual designer
    /// </remarks>
    /// <returns>
    /// <see cref="AppBuilder"/>
    /// </returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}