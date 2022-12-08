namespace FluentPaint.Core.Huffman;

public class HuffmanTree
{
    private readonly byte[] _numbersOfSymbols;
    private readonly byte[] _symbolCodes;

    public HuffmanTree(byte header, byte[] numbersOfSymbols, byte[] symbolCodes)
    {
        _numbersOfSymbols = numbersOfSymbols;
        _symbolCodes = symbolCodes;

        Create();
    }

    public readonly Node Root = new();

    private void Create()
    {
        var pointer = 0;
        
        for (var number = 0; number < _numbersOfSymbols.Length; number++)
        {
            for (var i = 0; i < _numbersOfSymbols[number]; i++)
            {
                GoDown(Root, 0, number, _symbolCodes[pointer++]);
            }
        }
    }

    private bool GoDown(Node node, int depth, int codeSize, int code)
    {
        if (depth == codeSize)
        {
            if (node.Left == null)
            {
                node.Left = new Node
                {
                    Value = node.Value + "0",
                    Code = code
                };

                return true;
            }

            if (node.Right == null)
            {
                node.Right = new Node
                {
                    Value = node.Value + "1",
                    Code = code
                };

                return true;
            }
        }

        depth += 1;

        if (node.Left is null)
        {
            node.Left = new Node
            {
                Value = node.Value + "0"
            };

            if (GoDown(node.Left, depth, codeSize, code))
            {
                return true;
            }
        }

        if (node.Left.Code == -1)
        {
            if (GoDown(node.Left, depth, codeSize, code))
            {
                return true;
            }
        }

        if (node.Right is null)
        {
            node.Right = new Node
            {
                Value = node.Value + "1"
            };

            if (GoDown(node.Right, depth, codeSize, code))
            {
                return true;
            }
        }

        if (node.Right.Code == -1)
        {
            if (GoDown(node.Right, depth, codeSize, code))
            {
                return true;
            }
        }

        return false;
    }
}