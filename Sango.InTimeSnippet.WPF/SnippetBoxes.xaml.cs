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
using System.Windows.Media;
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

    private void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
        var input = Input.Text;
        var output = _snippetEngine.ExpandText(input);
        Output.Text = output;
        LaTeX.Formula = output;
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
}
