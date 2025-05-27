using System.ComponentModel.DataAnnotations;
using ByteCart.Application.Suppliers.Commands.CreateSupplier;
using ByteCart.Application.Suppliers.Commands.UpdateSupplier;

namespace Presentation.ViewModels;

public class SupplierViewModel
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string? ContactEmail { get; set; }
    [Required] public string? ContactNumber { get; set; }
    [Required] public string? Website { get; set; }
    public string? Id { get; set; }

    public CreateSupplierCommand CreateCommand() => new CreateSupplierCommand
    {
        Name = Name, ContactEmail = ContactEmail, ContactNumber = ContactNumber, Website = Website
    };

    public UpdateSupplierCommand UpdateCommand() => new UpdateSupplierCommand
    {
        Id = Id!,
        Name = Name,
        ContactEmail = ContactEmail,
        ContactNumber = ContactNumber,
        Website = Website
    };
}
