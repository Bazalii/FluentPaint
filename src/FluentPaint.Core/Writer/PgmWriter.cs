using SkiaSharp;

namespace FluentPaint.Core.Writer;

public class PgmWriter : IPnmWriter
{
    public void WriteImageData(FileStream fs, SKBitmap im)
    {
        for (var y = 0; y < im.Height; y++)
        {
            for (var x = 0; x < im.Width; x++)
            {
                var c = im.GetPixel(x, y);
                var luma = (int) (c.Red * 0.3 + c.Green * 0.59 + c.Blue * 0.11);
                fs.WriteByte((byte) luma);
            }
        }

        fs.Close();
    }
}