using ByteCart.Application.Common.Models;
using ByteCart.Application.Suppliers;
using ByteCart.Application.Suppliers.Commands.DeleteSupplier;
using ByteCart.Application.Suppliers.Queries.GetSuppliers;
using Presentation.ViewModels;

namespace Presentation.Components.Pages.Suppliers;

public partial class Suppliers
{
    private PaginatedList<SupplierDto>? suppliers;
    private bool loading = true;
    private string alertMessage = string.Empty;
    private string alertType = "info";

    // Search parameters
    private string searchTerm = string.Empty;

    // Pagination 
    private int currentPage = 1;
    private int pageSize = 10;

    // Modal state
    private bool showSupplierModal = false;
    private string modalTitle = "Create Supplier";
    private SupplierViewModel _supplierViewModel = new();

    // Delete confirmation
    private bool showDeleteConfirmation = false;
    private string supplierToDeleteId = string.Empty;
    private string supplierToDeleteName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadSuppliersAsync();
    }

    private async Task LoadSuppliersAsync()
    {
        loading = true;
        try
        {
            var query = new GetSuppliersQuery
            {
                PageNumber = currentPage,
                PageSize = pageSize,
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm
            };

            suppliers = await Mediator.Send(query);
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading suppliers: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            loading = false;
        }
    }

    private async Task ApplySearch()
    {
        currentPage = 1;
        await LoadSuppliersAsync();
    }

    private async Task OnPageChangeAsync(int page)
    {
        currentPage = page;
        await LoadSuppliersAsync();
    }

    private void OpenCreateSupplierModal()
    {
        _supplierViewModel = new SupplierViewModel();
        modalTitle = "Create Supplier";
        showSupplierModal = true;
    }

    private void OpenEditSupplierModal(SupplierDto supplier)
    {
        _supplierViewModel = new SupplierViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            ContactNumber = supplier.ContactNumber,
            Website = supplier.Website
        };
        modalTitle = "Edit Supplier";
        showSupplierModal = true;
    }

    private void CloseModal()
    {
        showSupplierModal = false;
    }

    private async Task SaveSupplier()
    {
        try
        {
            if (string.IsNullOrEmpty(_supplierViewModel.Id))
            {
                // Create new supplier
                await Mediator.Send(_supplierViewModel.CreateCommand());
                alertMessage = "Supplier created successfully";
            }
            else
            {
                // Update existing supplier
                await Mediator.Send(_supplierViewModel.UpdateCommand());
                alertMessage = "Supplier updated successfully";
            }

            alertType = "success";
            showSupplierModal = false;
            await LoadSuppliersAsync();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error saving supplier: {ex.Message}";
            alertType = "danger";
        }
    }

    private void ShowDeleteConfirmation(SupplierDto supplier)
    {
        supplierToDeleteId = supplier.Id;
        supplierToDeleteName = supplier.Name;
        showDeleteConfirmation = true;
    }

    private void CancelDelete()
    {
        showDeleteConfirmation = false;
    }

    private async Task DeleteSupplier()
    {
        try
        {
            await Mediator.Send(new DeleteSupplierCommand { Id = supplierToDeleteId });
            alertMessage = "Supplier deleted successfully";
            alertType = "success";
            showDeleteConfirmation = false;
            await LoadSuppliersAsync();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error deleting supplier: {ex.Message}";
            alertType = "danger";
        }
    }
}
