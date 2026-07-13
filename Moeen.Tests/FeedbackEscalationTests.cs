using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;

namespace Moeen.Tests;

public class FeedbackEscalationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task SupervisorTransfer_MovesComplaintAndSuggestionToOwnerView()
    {
        await using var context = CreateContext();
        var complaint = NewFeedback(FeedbackType.Complaint);
        var suggestion = NewFeedback(FeedbackType.Suggestion);
        context.Complaints.AddRange(complaint, suggestion);
        await context.SaveChangesAsync();

        var supervisorService = CreateService(context, "Supervisor");

        var complaintTransfer = await supervisorService.TransferToOwnerAsync(complaint.Id);
        var suggestionTransfer = await supervisorService.TransferToOwnerAsync(suggestion.Id);

        Assert.True(complaintTransfer.Success);
        Assert.True(suggestionTransfer.Success);
        Assert.Equal(ComplaintStatus.TransferredToOwner, complaint.Status);
        Assert.Equal(SuggestionStatus.TransferredToOwner, suggestion.SuggestionStatus);
        Assert.NotNull(complaint.UpdatedAt);
        Assert.NotNull(suggestion.UpdatedAt);

        var supervisorComplaints = await supervisorService.GetComplaintsAsync(new PaginationRequest { Page = 1, PageSize = 50 });
        var supervisorSuggestions = await supervisorService.GetSuggestionsAsync(new PaginationRequest { Page = 1, PageSize = 50 });
        Assert.Empty(ReadItems(supervisorComplaints.Data));
        Assert.Empty(ReadItems(supervisorSuggestions.Data));

        var ownerService = CreateService(context, "Owner");
        var ownerComplaints = await ownerService.GetComplaintsAsync(new PaginationRequest { Page = 1, PageSize = 50 });
        var ownerSuggestions = await ownerService.GetSuggestionsAsync(new PaginationRequest { Page = 1, PageSize = 50 });

        Assert.Contains(ReadItems(ownerComplaints.Data), item => item.Id == complaint.Id);
        Assert.Contains(ReadItems(ownerSuggestions.Data), item => item.Id == suggestion.Id);
    }

    [Fact]
    public async Task SupervisorCannotDeleteFeedback()
    {
        await using var context = CreateContext();
        var complaint = NewFeedback(FeedbackType.Complaint);
        context.Complaints.Add(complaint);
        await context.SaveChangesAsync();

        var supervisorService = CreateService(context, "Supervisor");
        var response = await supervisorService.DeleteComplaintAsync(complaint.Id);

        Assert.False(response.Success);
        Assert.NotNull(await context.Complaints.FindAsync(complaint.Id));
    }

    [Fact]
    public void SupervisorFeedbackPage_HidesDeleteAndRefreshAndShowsTransfer()
    {
        var source = File.ReadAllText(Path.Combine(FindRepositoryRoot(),
            "Moeen.Dashboard/Components/Pages/Supervisor/ComplaintAndSuggestions.razor"));

        Assert.DoesNotContain("DeleteFeedbackAsync(item.Id)", source);
        Assert.DoesNotContain("title=\"حذف\"", source);
        Assert.DoesNotContain("bi-arrow-clockwise", source);
        Assert.Contains("TransferToOwnerAsync(item)", source);
        Assert.Contains("تحويل للمالك", source);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static FeedbackService CreateService(AppDbContext context, params string[] roles)
        => new(new UnitOfWork(context), new FakeCurrentUserService(Guid.NewGuid(), roles));

    private static Complaint NewFeedback(FeedbackType type)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = type == FeedbackType.Suggestion ? "اقتراح اختباري" : null,
            content = type == FeedbackType.Complaint ? "شكوى اختبارية" : "محتوى اقتراح اختباري",
            created_at = DateTime.UtcNow,
            Type = type,
            Status = type == FeedbackType.Complaint ? ComplaintStatus.InProgress : null,
            SuggestionStatus = type == FeedbackType.Suggestion ? SuggestionStatus.InProgress : null
        };

    private static List<ComplaintDto> ReadItems(object? data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        return JsonSerializer.Deserialize<List<ComplaintDto>>(json, JsonOptions) ?? new List<ComplaintDto>();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Moeen.sln")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Moeen repository root was not found.");
    }

    private sealed class FakeCurrentUserService(Guid userId, IEnumerable<string> roles) : ICurrentUserService
    {
        private readonly HashSet<string> _roles = new(roles, StringComparer.OrdinalIgnoreCase);

        public Guid? CurrentUserId => userId;
        public string CurrentUserName => "feedback-test-user";
        public bool? IsActived => true;
        public bool? IsAdmin => _roles.Contains("Admin") || _roles.Contains("Owner");
        public bool IsInRole(string roleName) => _roles.Contains(roleName);
        public string GetBaseUrl(string relativePath) => relativePath;
    }
}
