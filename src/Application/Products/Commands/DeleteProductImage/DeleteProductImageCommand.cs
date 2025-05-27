using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ByteCart.Application.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand : IRequest<bool>
{
    public string ProductId { get; init; } = null!;
    public string ImageId { get; init; } = null!;
}

public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public DeleteProductImageCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<bool> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == request.ImageId && i.ProductId == request.ProductId, cancellationToken);

        if (image == null)
        {
            throw new NotFoundException(nameof(ProductImage), request.ImageId);
        }

        // Delete the physical file
        _fileService.DeleteFile(image.Url);

        // Remove from database
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
