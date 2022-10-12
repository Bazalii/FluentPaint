using SkiaSharp;

namespace FluentPaint.Core.Writer;

public class PpmWriter : IPnmWriter
{
    public void WriteImageData(FileStream fs, SKBitmap im)
    {
        for (var y = 0; y < im.Height; y++)
        {
            for (var x = 0; x < im.Width; x++)
            {
                SKColor c = im.GetPixel(x, y);
                fs.WriteByte(c.Red);
                fs.WriteByte(c.Green);
                fs.WriteByte(c.Blue);
            }
        }

        fs.Close();
    }
}