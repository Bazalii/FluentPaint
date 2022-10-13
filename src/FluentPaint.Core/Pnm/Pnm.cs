using System.Text;
using SkiaSharp;

namespace FluentPaint.Core.Pnm;

public class Pnm
{
    public SKBitmap ReadPnm(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var magicWord = (char) fs.ReadByte() + ((char) fs.ReadByte()).ToString();
        fs.ReadByte();
        
        var line = ReadLine(fs);
        var parameters = line.Split(new[]{' '});
        var width = int.Parse(parameters[0]);
        var height = int.Parse(parameters[1]);
        
        var brightness = int.Parse(ReadLine(fs));

        var imReader = PnmFactory.GetIPnmReader(PnmFactory.GetPnmType(magicWord));
        return imReader.ReadImageData(fs, width, height);
    }

    public void WritePnm(string filePath, SKBitmap bitmap)
    {
        var extension = filePath.Substring(filePath.Length-4,4).ToLower();
        var type = extension switch
        {
            ".pgm" => PnmType.P5,
            ".ppm" => PnmType.P6,
            ".pnm" => PnmType.P6,
            _ => throw new Exception()
        };
        
        var fs = new FileStream(filePath, FileMode.Create,FileAccess.Write,FileShare.None);
        
        WriteLine(fs,type.ToString());				
        WriteLine(fs,bitmap.Width + " " + bitmap.Height);
        WriteLine(fs,"255");

        var imWriter = PnmFactory.GetIPnmWriter(type);
        imWriter.WriteImageData(fs, bitmap);
    }

    private static string ReadLine(FileStream fs)
    {
        var br = new BinaryReader(fs);

        var sb = new StringBuilder();

        var cur = br.ReadByte();
        while (cur != '\n')
        {
            sb.Append(((char) cur).ToString());
            cur = br.ReadByte();
        }

        return sb.ToString();
    }
    
    private void WriteLine(FileStream fs, string line)
    {
        var bw = new BinaryWriter(fs);
        
        bw.Write(Encoding.ASCII.GetBytes(line+"\n"));			
    }
}