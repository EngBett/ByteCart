using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Tags.Commands.CreateTag;

public record CreateTagCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
}

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (name, cancellation) =>
                !await context.Tags.AnyAsync(t => t.Name == name, cancellation))
            .WithMessage("A tag with this name already exists.");
    }
}

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateTagCommandHandler(IApplicationDbContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag { Name = request.Name };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }
}
