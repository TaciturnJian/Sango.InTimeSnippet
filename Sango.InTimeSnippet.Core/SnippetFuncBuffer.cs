namespace Sango.InTimeSnippet.Core;

public record class SnippetFuncBuffer(string Text = "")
{
    public string Text { get; set; } = Text;

    public static readonly string ArgPlaceHolder = "@@";

    public bool ContainsArgPlace => Text.Contains(ArgPlaceHolder);

    public int FirstArgPlace => Text.IndexOf(ArgPlaceHolder);

    public bool InsertArg(string arg)
    {
        var place = FirstArgPlace;
        if (place < 0 || place >= Text.Length)
            return false;

        var left = Text.AsSpan(0, place);
        var right = Text.AsSpan(place + ArgPlaceHolder.Length);
        Text = string.Concat(left, arg, right);
        return true;
    }

    public void CleanArgs()
    {
        while (ContainsArgPlace)
        {
            InsertArg("");
        }
    }

    public override string ToString()
    {
        return Text;
    }
}
