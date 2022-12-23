namespace FluentPaint.UI.Models;

public class FilterParameters
{
    public float Sigma { get; set; }
    public float Sharpness { get; set; }
    public int Radius { get; set; }
    public int Limit { get; set; }
    public byte Threshold { get; set; }
}