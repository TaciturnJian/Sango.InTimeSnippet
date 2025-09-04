namespace Sango.InTimeSnippet.Core;

public sealed class ProgramMutex : IDisposable
{
    public string MutexName { get; private set; } = "Sango.InTimeSnippet.Core";

    private Mutex? _mutex;

    public bool Succeeded => _mutex != null;

    public bool IsDisposed { get; private set; }

    private void CleanMutex()
    {
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        _mutex = null;
    }

    public ProgramMutex(string name)
    {
        MutexName = name;

        try
        {
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
                CleanMutex();

        }
        catch (Exception)
        {
            CleanMutex();
            return;
        }
    }

    private void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;

        if (disposing)
            CleanMutex();

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
