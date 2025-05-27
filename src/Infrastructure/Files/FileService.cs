using ByteCart.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ByteCart.Infrastructure.Files;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsFolder;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
        
        // Ensure directory exists
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null)
        {
            throw new ArgumentException("File is empty or null", nameof(file));
        }

        // Generate a unique file name to prevent overwriting existing files
        string fileExtension = Path.GetExtension(file.FileName);
        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        string filePath = Path.Combine(_uploadsFolder, fileName);
        
        // Save the file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the relative path for storing in the database
        return $"/uploads/products/{fileName}";
    }

    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
        {
            return;
        }

        // Convert relative URL to absolute file path
        string fileName = Path.GetFileName(fileUrl);
        string filePath = Path.Combine(_uploadsFolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
