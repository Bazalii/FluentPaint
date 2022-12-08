namespace FluentPaint.Core.Huffman;

public class Node
{
    public Node? Left { get; set; }
    public Node? Right { get; set; }
    public string Value { get; init; } = string.Empty;
    public int Code { get; init; } = -1;
}