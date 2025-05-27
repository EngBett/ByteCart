using Microsoft.AspNetCore.Http;

namespace ByteCart.Application.Products.Commands.UploadProductImage;

public class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    
    public UploadProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");
            
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required")
            .Must(file => file == null || file.Length <= 10 * 1024 * 1024)
            .WithMessage("Image size must be less than 10MB")
            .Must(HaveValidExtension)
            .WithMessage($"Image must be one of the following formats: {string.Join(", ", _allowedExtensions)}");
    }
    
    private bool HaveValidExtension(IFormFile file)
    {
        if (file == null) return true;
        
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}
