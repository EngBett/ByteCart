using Microsoft.AspNetCore.Http;

namespace ByteCart.Application.Common.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
    void DeleteFile(string fileUrl);
}
