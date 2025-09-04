using Sango.InTimeSnippet.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sango.InTimeSnippet.WPF;

/// <summary>
/// SnippetBoxes.xaml 的交互逻辑
/// </summary>
public partial class SnippetBoxes : UserControl
{
    public SnippetBoxes()
    {
        InitializeComponent();

        LoadSnippets();
    }

    private SnippetEngine _snippetEngine = new();

    public bool LaTeXVisible => LaTeX.Visibility == Visibility.Visible;

    public void HideLaTeX()
    {
        LaTeX.Visibility = Visibility.Collapsed;
    }

    public void ShowLaTeX()
    {
        LaTeX.Visibility = Visibility.Visible;
    }

    public void TurnLaTeX()
    {
        if (LaTeXVisible)
        {
            HideLaTeX();
        }
        else
        {
            ShowLaTeX();
        }
    }

    private bool _is_updating = false;

    private static string GetStr(FlowDocument doc)
    {
        var text_range = new TextRange(
            doc.ContentStart,
            doc.ContentEnd);
        return text_range.Text;
    }

    private void SetColorBox()
    {
        var begin = ColorBox.Document.ContentStart;
        var end = ColorBox.Document.ContentEnd;
        var left = begin;
        var right = begin;

        var it = new BrushIterators();
        var count_stack = new Stack<int>();
        var brush = it.Current;

        while (right != end)
        {
            var text_range = new TextRange(left, right);
            var text = text_range.Text;
            if (text.EndsWith(' ') || text.EndsWith('\n') || text.EndsWith('\t'))
            {
                var expanded = _snippetEngine.ExpandWord(text.Trim());
                var func = new SnippetFuncBuffer(expanded);
                var arg_count = func.GetArgPlaceCount();

                if (arg_count > 0)
                {
                    count_stack.Push(arg_count);
                    brush = it.Next();
                }
                else
                {
                    if (count_stack.TryPop(out var count))
                    {
                        if (count > 0)
                        {
                            count--;
                            count_stack.Push(count);
                        }
                        else
                        {
                            brush = it.Last();
                        }
                    }
                }

                text_range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                left = right;
            }

            right = right.GetPositionAtOffset(1, LogicalDirection.Forward);
            if (right is null)
                break;
        }


    }

    private void LoadSnippets()
    {
        var file = "snippets.txt";
        try
        {
            var lines = System.IO.File.ReadAllLines(file);
            foreach (var line in lines)
            {
                _snippetEngine.LoadFromLine(line);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"从文件({file})加载 Snippets 失败: {ex.Message}");
        }
    }

    private void ColorBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_is_updating)
            return;

        var text_range = new TextRange(
            ColorBox.Document.ContentStart,
            ColorBox.Document.ContentEnd);

        var input = GetStr(ColorBox.Document);
        var input_trim = input.Trim();
        var output = _snippetEngine.ExpandText(input_trim);
        Output.Text = output;
        LaTeX.Formula = output;
        _is_updating = true;
        SetColorBox();
        _is_updating = false;
    }
}
