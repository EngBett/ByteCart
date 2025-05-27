using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ByteCart.Presentation.Extensions;

public static class FileExtensions
{
    public static async Task<IFormFile> ConvertToFormFileAsync(this IBrowserFile browserFile, long maxAllowedSize = 5242880)
    {
        using var stream = browserFile.OpenReadStream(maxAllowedSize);
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;
        
        // Create a custom implementation that doesn't dispose the MemoryStream
        return new BrowserFormFile(ms, browserFile.Name, browserFile.ContentType);
    }
    
    // Custom implementation of IFormFile that works with MemoryStream
    private class BrowserFormFile : IFormFile
    {
        private readonly MemoryStream _stream;
        
        public BrowserFormFile(MemoryStream stream, string fileName, string contentType)
        {
            _stream = stream;
            Name = FileName = fileName;
            ContentType = contentType;
            Length = stream.Length;
        }
        
        public string ContentType { get; }
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
        public IHeaderDictionary Headers { get; } = new HeaderDictionary();
        public long Length { get; }
        public string Name { get; }
        public string FileName { get; }
        
        public Stream OpenReadStream() => _stream;
        
        public void CopyTo(Stream target) => _stream.CopyTo(target);
        
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            await _stream.CopyToAsync(target, cancellationToken);
        }
    }
}
