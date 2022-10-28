using System.Text;
using SkiaSharp;

namespace FluentPaint.Core.Pnm;

public class Pnm
{
    public static SKBitmap ReadPnm(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var type = ReadLine(fileStream);

        var line = ReadLine(fileStream);
        var parameters = line.Split(new[] { ' ' });
        var width = int.Parse(parameters[0]);
        var height = int.Parse(parameters[1]);

        var brightness = int.Parse(ReadLine(fileStream));

        if (brightness != 255)
        {
            throw new Exception("Brightness should be 255!");
        }

        var reader = PictureFactory.GetReader(PictureFactory.GetType(type));

        return reader.ReadImageData(fileStream, width, height);
    }

    public static void WritePnm(string filePath, SKBitmap bitmap)
    {
        var extension = filePath.Substring(filePath.Length - 4, 4).ToLower();

        var type = extension switch
        {
            ".pgm" => PictureType.P5,
            ".ppm" => PictureType.P6,
            ".pnm" => PictureType.P6,
            _ => throw new Exception(
                "Error: This file type is not supported, .ppm .pgm is expected (.pnm will write as p6)")
        };

        var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var writer = PictureFactory.GetWriter(type);

        WriteLine(fileStream, type.ToString());
        WriteLine(fileStream, bitmap.Width + " " + bitmap.Height);
        WriteLine(fileStream, "255");

        writer.WriteImageData(fileStream, bitmap);
    }

    private static string ReadLine(Stream fileStream)
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

    private static void WriteLine(Stream fileStream, string line)
    {
        var binaryWriter = new BinaryWriter(fileStream);

        binaryWriter.Write(Encoding.ASCII.GetBytes(line + "\n"));
    }
}