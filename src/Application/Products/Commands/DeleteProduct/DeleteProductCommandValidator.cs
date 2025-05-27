using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required")
            .MustAsync(async (id, cancellation) =>
                await dbContext.Products.AnyAsync(p => p.Id == id, cancellation))
            .WithMessage("Product not found");
    }
}
