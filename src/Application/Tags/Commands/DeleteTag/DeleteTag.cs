using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
}

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await context.Tags.AnyAsync(t => t.Id == id, cancellation))
            .WithMessage("Tag does not exist.");
    }
}

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, string>
{
    private readonly IApplicationDbContext _context;

    public DeleteTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .Include(t => t.Products)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tag == null)
            throw new NotFoundException(nameof(Tag), request.Id);

        if (tag.Products.Any())
            throw new ValidationException("Cannot delete tag assigned to one or more products.");

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }
}
