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

    public string ExpandText(string input)
    {
        if (input.Length == 0)
            return "";

        var words = input.Split([' ', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries);

        var func_stack = new Stack<SnippetFuncBuffer>();

        var sb = new StringBuilder();
        foreach (var word in words)
        {
            var expanded_word = ExpandWord(word);
            var func_buffer = new SnippetFuncBuffer(expanded_word);

            if (func_buffer.ContainsArgPlace)
            {
                func_stack.Push(func_buffer);
                continue;
            }

            var arg_text = func_buffer.Text;
            while (true)
            {
                if (func_stack.Count == 0)
                {
                    sb.Append(arg_text);
                    sb.Append(' ');
                    break;
                }

                var buffer = func_stack.Pop();
                buffer.InsertArg(arg_text);
                if (buffer.ContainsArgPlace)
                {
                    func_stack.Push(buffer);
                    break;
                }

                arg_text = buffer.Text;
            }
        }

        if (func_stack.Count > 0)
        {
            string? pre_text = null;
            while (func_stack.Count > 1)
            {
                var func = func_stack.Pop();
                if (!func.ContainsArgPlace)
                {
                    if (pre_text is not null)
                    {
                        sb.Insert(0, ' ');
                        sb.Insert(0, pre_text);
                    }
                    pre_text = func.Text;
                    continue;
                }

                if (pre_text is null)
                {
                    func.CleanArgs();
                    pre_text = func.Text;
                    continue;
                }
                func.InsertArg(pre_text);
                func.CleanArgs();
                pre_text = func.Text;
            }

            var first_func = func_stack.Pop();
            if (pre_text is not null)
            {
                first_func.InsertArg(pre_text);
            }
            first_func.CleanArgs();
            sb.Insert(0, ' ');
            sb.Insert(0, first_func.Text);
        }

        return sb.ToString().Replace("__", "");
    }

    public void LoadFromLine(string line)
    {
        var comment = '#';
        var end_sentence = ';';
        var begin_word = '`';
        var end_word = '`';
        var assign = '=';
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
