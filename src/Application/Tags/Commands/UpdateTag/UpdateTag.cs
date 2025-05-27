using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Tags.Commands.UpdateTag;

public record UpdateTagCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, string>
{
    private readonly IApplicationDbContext _context;

    public UpdateTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags.FindAsync(new object[] { request.Id }, cancellationToken);
        if (tag == null)
            throw new NotFoundException(nameof(Tag), request.Id);

        tag.Name = request.Name;
        tag.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return tag.Id;
    }
}
