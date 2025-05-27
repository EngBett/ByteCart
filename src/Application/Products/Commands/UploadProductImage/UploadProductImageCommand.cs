using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ByteCart.Application.Products.Commands.UploadProductImage;

public record UploadProductImageCommand : IRequest<string>
{
    public string ProductId { get; init; } = null!;
    public IFormFile Image { get; init; } = null!;
}

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public UploadProductImageCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<string> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        // Find the product
        var product = await _context.Products
            .FindAsync([request.ProductId], cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        // Save the file and get the URL
        var imageUrl = await _fileService.SaveFileAsync(request.Image);

        // Create a new ProductImage entity
        var productImage = new ProductImage
        {
            ProductId = product.Id,
            Url = imageUrl
        };

        // Add to database
        _context.ProductImages.Add(productImage);
        await _context.SaveChangesAsync(cancellationToken);

        return imageUrl;
    }
}
