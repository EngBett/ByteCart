using ByteCart.Application.Tags;
using ByteCart.Application.Tags.Commands.CreateTag;
using ByteCart.Application.Tags.Commands.DeleteTag;
using ByteCart.Application.Tags.Commands.UpdateTag;
using ByteCart.Application.Tags.Queries.GetTags;
using Presentation.ViewModels;

namespace Presentation.Components.Pages.Tags;

public partial class Tags
{
    private List<TagDto>? tags;
    private bool loading = true;
    private string alertMessage = string.Empty;
    private string alertType = "info";
    
    // Modal state
    private bool showTagModal = false;
    private string modalTitle = "Create Tag";
    private TagViewModel tagModel = new();
    
    // Delete confirmation
    private bool showDeleteConfirmation = false;
    private string tagToDeleteId = string.Empty;
    private string tagToDeleteName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTagsAsync();
    }
    
    private async Task LoadTagsAsync()
    {
        loading = true;
        try
        {
            var result = await Mediator.Send(new GetTagsQuery());
            tags = result.ToList();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading tags: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            loading = false;
        }
    }
    
    private void OpenCreateTagModal()
    {
        tagModel = new TagViewModel();
        modalTitle = "Create Tag";
        showTagModal = true;
    }
    
    private void OpenEditTagModal(TagDto tag)
    {
        tagModel = new TagViewModel
        {
            Id = tag.Id,
            Name = tag.Name
        };
        modalTitle = "Edit Tag";
        showTagModal = true;
    }
    
    private void CloseModal()
    {
        showTagModal = false;
    }
    
    private async Task SaveTag()
    {
        try
        {
            if (string.IsNullOrEmpty(tagModel.Id))
            {
                // Create new tag
                var command = new CreateTagCommand
                {
                    Name = tagModel.Name
                };
                
                await Mediator.Send(command);
                alertMessage = "Tag created successfully";
            }
            else
            {
                // Update existing tag
                var command = new UpdateTagCommand
                {
                    Id = tagModel.Id,
                    Name = tagModel.Name
                };
                
                await Mediator.Send(command);
                alertMessage = "Tag updated successfully";
            }
            
            alertType = "success";
            showTagModal = false;
            await LoadTagsAsync();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error saving tag: {ex.Message}";
            alertType = "danger";
        }
    }
    
    private void ShowDeleteConfirmation(string id, string name)
    {
        tagToDeleteId = id;
        tagToDeleteName = name;
        showDeleteConfirmation = true;
    }
    
    private void CancelDelete()
    {
        showDeleteConfirmation = false;
    }
    
    private async Task DeleteTag()
    {
        try
        {
            await Mediator.Send(new DeleteTagCommand { Id = tagToDeleteId });
            alertMessage = "Tag deleted successfully";
            alertType = "success";
            showDeleteConfirmation = false;
            await LoadTagsAsync();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error deleting tag: {ex.Message}";
            alertType = "danger";
        }
    }
}
