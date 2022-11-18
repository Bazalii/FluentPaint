using FluentPaint.Core.Pictures.HeaderReaders.Models;

namespace FluentPaint.Core.Pictures.HeaderReaders;

public interface IHeaderReader
{
    PictureSize Read(Stream fileStream);
}