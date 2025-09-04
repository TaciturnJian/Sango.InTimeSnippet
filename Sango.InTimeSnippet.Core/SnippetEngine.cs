using System.Text;

namespace Sango.InTimeSnippet.Core;

public class SnippetEngine
{
    public Dictionary<string, string> Snippets { get; set; } = [];

    public string ExpandWord(string word)
    {
        string buffer = word;
        string prefix = "";
        while (buffer.Length > 0)
        {
            if (Snippets.TryGetValue(buffer, out var snippet))
                return prefix + snippet;
            prefix += buffer[0];
            buffer = buffer[1..];
        }
        return word;
    }

    private static void OutputStrStack(Stack<string> stack, StringBuilder sb, bool reverse = false)
    {
        foreach (var str in reverse ? stack.Reverse() : stack)
        {
            sb.Insert(0, ' ');
            sb.Insert(0, str);
        }
        stack.Clear();
    }

    private static void ApplyArgStack(Stack<string> stack, SnippetFuncBuffer func)
    {
        while (stack.Count > 0)
        {
            var arg = stack.Pop();
            func.InsertArg(arg);
            if (!func.ContainsArgPlace)
                break;
        }
    }

    public string ExpandText(string input)
    {
        const string placeholder = "__";

        if (input.Length == 0)
            return "";

        var words = input.Split([' ', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries);

        var func_stack = new Stack<SnippetFuncBuffer>();

        var sb = new StringBuilder();
        foreach (var word in words)
            func_stack.Push(new(ExpandWord(word)));

        if (func_stack.Count == 0)
            return "";

        var arg_stack = new Stack<string>();
        while (func_stack.Count > 0)
        {
            var func = func_stack.Pop();
            if (!func.ContainsArgPlace)
            {
                arg_stack.Push(func.Text);
                continue;
            }

            ApplyArgStack(arg_stack, func);
            func.CleanArgs();
            arg_stack.Push(func.Text);
        }

        OutputStrStack(arg_stack, sb, true);

        return sb.ToString().Replace(placeholder, "");
    }

    public void LoadFromLine(string line)
    {
        const char comment = '#';
        const char end_sentence = ';';
        const char begin_word = '`';
        const char end_word = '`';
        const char assign = '=';
        var pattern = @$"{begin_word}(.*?){end_word}\s*{assign}\s*{begin_word}(.*?){end_word}{end_sentence}";

        if (line.StartsWith(comment))
            return;

        var matches = System.Text.RegularExpressions.Regex.Matches(line, pattern);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (match.Groups.Count == 3)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                Snippets[key] = value;
            }
        }
    }

    public void LoadFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            LoadFromLine(line);
        }
    }
}
