using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moeen.Api.Controllers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.OwnerDashboard;

namespace Moeen.Tests;

public class OwnerDashboardFeedbackScopeTests
{
    [Fact]
    public async Task Overview_ShowsOnlyFeedbackTransferredToOwner()
    {
        await using var context = CreateContext();
        var now = DateTime.UtcNow;

        context.Complaints.AddRange(
            NewFeedback("شكوى محولة ظاهرة", FeedbackType.Complaint, now.AddMinutes(-6),
                complaintStatus: ComplaintStatus.TransferredToOwner, updatedAt: now.AddMinutes(-2)),
            NewFeedback("اقتراح محول ظاهر", FeedbackType.Suggestion, now.AddMinutes(-5),
                suggestionStatus: SuggestionStatus.TransferredToOwner, updatedAt: now.AddMinutes(-1)),
            NewFeedback("شكوى غير محولة مخفية", FeedbackType.Complaint, now.AddMinutes(-4),
                complaintStatus: ComplaintStatus.InProgress),
            NewFeedback("اقتراح غير محول مخفي", FeedbackType.Suggestion, now.AddMinutes(-3),
                suggestionStatus: SuggestionStatus.InProgress),
            NewFeedback("شكوى محلولة مخفية", FeedbackType.Complaint, now.AddMinutes(-2),
                complaintStatus: ComplaintStatus.Resolved),
            NewFeedback("اقتراح محلول مخفي", FeedbackType.Suggestion, now.AddMinutes(-1),
                suggestionStatus: SuggestionStatus.Resolved));

        await context.SaveChangesAsync();

        var response = await GetOverviewAsync(context);

        Assert.Equal(2, response.TotalOpenComplaints);
        Assert.Contains(response.RecentActivities, item =>
            item.Title == "شكوى محولة" && item.Description == "شكوى محولة ظاهرة");
        Assert.Contains(response.RecentActivities, item =>
            item.Title == "اقتراح محول" && item.Description == "اقتراح محول ظاهر");
        Assert.DoesNotContain(response.RecentActivities, item => item.Description.Contains("غير محول"));
        Assert.DoesNotContain(response.RecentActivities, item => item.Description.Contains("محلول"));
    }

    [Fact]
    public async Task MosqueSummary_CountsOnlyTransferredFeedbackAndKeepsPostActivity()
    {
        await using var context = CreateContext();
        var now = DateTime.UtcNow;
        var mosque = new Mosque
        {
            Id = Guid.NewGuid(),
            name = "مسجد الاختبار",
            address = "عنوان اختباري",
            contact_phone = string.Empty,
            Description = string.Empty
        };
        var supervisor = new Supervisor
        {
            Id = Guid.NewGuid(),
            MosqueId = mosque.Id,
            Mosque = mosque,
            UserName = "owner-dashboard-supervisor",
            name = "مشرف اختبار",
            role = (int)Roles.Admin,
            created_at = now,
            JoinedAt = now,
            assigned_at = now
        };

        context.Mosques.Add(mosque);
        context.Supervisors.Add(supervisor);
        context.Complaints.AddRange(
            NewFeedback("شكوى المسجد المحولة", FeedbackType.Complaint, now.AddMinutes(-5), supervisor.Id,
                ComplaintStatus.TransferredToOwner, updatedAt: now.AddMinutes(-2)),
            NewFeedback("اقتراح المسجد المحول", FeedbackType.Suggestion, now.AddMinutes(-4), supervisor.Id,
                suggestionStatus: SuggestionStatus.TransferredToOwner, updatedAt: now.AddMinutes(-1)),
            NewFeedback("شكوى المسجد غير المحولة", FeedbackType.Complaint, now.AddMinutes(-3), supervisor.Id,
                ComplaintStatus.InProgress));
        context.Posts.Add(new Post
        {
            Id = Guid.NewGuid(),
            MosqueId = mosque.Id,
            Mosque = mosque,
            title = "منشور يبقى في النشاطات",
            body = "محتوى المنشور",
            imageUrl = string.Empty,
            created_at = now
        });

        await context.SaveChangesAsync();

        var response = await GetOverviewAsync(context);
        var mosqueSummary = Assert.Single(response.MosqueSummaries);

        Assert.Equal(2, mosqueSummary.OpenComplaints);
        Assert.Contains(response.RecentActivities, item => item.Title == "منشور يبقى في النشاطات");
        Assert.Contains(response.RecentActivities, item => item.Description == "شكوى المسجد المحولة");
        Assert.Contains(response.RecentActivities, item => item.Description == "اقتراح المسجد المحول");
        Assert.DoesNotContain(response.RecentActivities, item => item.Description == "شكوى المسجد غير المحولة");
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<OwnerDashboardResponse> GetOverviewAsync(AppDbContext context)
    {
        var controller = new OwnerDashboardController(
            context,
            NullLogger<OwnerDashboardController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetOverview();
        return Assert.IsType<OwnerDashboardResponse>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    private static Complaint NewFeedback(
        string content,
        FeedbackType type,
        DateTime createdAt,
        Guid? userId = null,
        ComplaintStatus? complaintStatus = null,
        SuggestionStatus? suggestionStatus = null,
        DateTime? updatedAt = null)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Title = type == FeedbackType.Suggestion ? "اقتراح اختباري" : null,
            content = content,
            created_at = createdAt,
            Type = type,
            Status = type == FeedbackType.Complaint ? complaintStatus : null,
            SuggestionStatus = type == FeedbackType.Suggestion ? suggestionStatus : null,
            UpdatedAt = updatedAt
        };
}
