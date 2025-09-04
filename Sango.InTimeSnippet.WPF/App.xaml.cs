using Sango.InTimeSnippet.Core;

using System.Configuration;
using System.Data;
using System.Windows;

namespace Sango.InTimeSnippet.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static readonly string MutexName = "Sango.InTimeSnippet.WPF";
    private static ProgramMutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        _mutex = new ProgramMutex(MutexName);
        if (!_mutex.Succeeded)
        {
            Shutdown();
            return;
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.Dispose();
        base.OnExit(e);
    }
}
