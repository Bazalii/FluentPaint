using SkiaSharp;

namespace FluentPaint.Core.Writer;

public class PgmWriter : IPnmWriter
{
    public void WriteImageData(FileStream fileStream, SKBitmap bitmap)
    {
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var luma = (int)(pixel.Red * 0.3 + pixel.Green * 0.59 + pixel.Blue * 0.11);
                fileStream.WriteByte((byte)luma);
            }
        }

        fileStream.Close();
    }
}