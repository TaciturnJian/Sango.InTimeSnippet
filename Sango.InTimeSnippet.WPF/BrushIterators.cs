using System.Windows.Media;

namespace Sango.InTimeSnippet.WPF;

public class BrushIterators
{
    private List<Brush> _brushes { get; set; } =
    [
        Brushes.Red,
        Brushes.Green,
        Brushes.DarkCyan,
        Brushes.Blue
    ];

    private int _index = 0;

    public Brush Current => _brushes[_index];

    public Brush Next()
    {
        _index = (_index + 1) % _brushes.Count;
        return Current;
    }

    public Brush Last()
    {
        _index = (_index - 1 + _brushes.Count) % _brushes.Count;
        return Current;
    }
}
