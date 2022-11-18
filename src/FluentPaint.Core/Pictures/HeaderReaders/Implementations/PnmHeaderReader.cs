using System.Text;
using FluentPaint.Core.Pictures.HeaderReaders.Models;

namespace FluentPaint.Core.Pictures.HeaderReaders.Implementations;

public class PnmHeaderReader : IHeaderReader
{
    public PictureSize Read(Stream fileStream)
    {
        ReadLine(fileStream);
        var line = ReadLine(fileStream);
        var parameters = line.Split(new[] { ' ' });
        var width = int.Parse(parameters[0]);
        var height = int.Parse(parameters[1]);

        var brightness = int.Parse(ReadLine(fileStream));

        if (brightness != 255)
        {
            throw new Exception("Brightness should be 255!");
        }

        return new PictureSize
        {
            Width = width,
            Height = height
        };
    }
    
    /// <summary>
    /// Reads line from provided filestream.
    /// </summary>
    /// <param name="fileStream"> <see cref="Stream"/> that contains information to read. </param>
    /// <returns>
    /// Line that was read from provided filestream.
    /// </returns>
    private string ReadLine(Stream fileStream)
    {
        var binaryReader = new BinaryReader(fileStream);
        var stringBuilder = new StringBuilder();
        var current = binaryReader.ReadByte();

        while (current != '\n')
        {
            stringBuilder.Append((char) current);
            current = binaryReader.ReadByte();
        }

        return stringBuilder.ToString();
    }
}