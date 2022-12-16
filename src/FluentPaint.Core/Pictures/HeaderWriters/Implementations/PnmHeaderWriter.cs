using System.Text;
using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.HeaderWriters.Implementations;

public class PnmHeaderWriter : IHeaderWriter
{
    public void Write(FileStream fileStream, FluentBitmap bitmap, PictureType type)
    {
        WriteLine(fileStream, type.ToString());
        WriteLine(fileStream, bitmap.Width + " " + bitmap.Height);
        WriteLine(fileStream, "255"); 
    }
    
    /// <summary>
    /// Writes line to provided filestream.
    /// </summary>
    /// <param name="fileStream"> <see cref="Stream"/> that is used for writing information. </param>
    /// <param name="line"> String that will be written to filestream. </param>
    public void WriteLine(Stream fileStream, string line)
    {
        var binaryWriter = new BinaryWriter(fileStream);

        binaryWriter.Write(Encoding.ASCII.GetBytes(line + "\n"));
    }
}