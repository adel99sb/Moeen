using Microsoft.EntityFrameworkCore;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses.Library;

namespace Moeen.Tests;

public class ExamerLibraryAccessTests
{
    [Fact]
    public async Task PlainExamer_SeesBooksWhenLibraryBelongsToOneMosque()
    {
        await using var db = CreateContext();
        var mosque = CreateMosque("Mosque A");
        var examiner = CreateExamer();
        var book = CreateBook("Examiner book", mosque.Id);

        db.AddRange(mosque, examiner, book);
        await db.SaveChangesAsync();

        var response = await CreateService(db, examiner.Id)
            .GetAllBooksAsync(new GetAllBooksRequest { PageNumber = 1, PageSize = 20 });
        var books = Assert.IsType<List<BookResponseDto>>(response.Data);

        Assert.True(response.Success);
        Assert.Single(books);
        Assert.Equal(book.title, books[0].Title);
    }

    [Fact]
    public async Task PlainExamer_DoesNotReceiveCrossMosqueBooksWhenScopeIsAmbiguous()
    {
        await using var db = CreateContext();
        var mosqueA = CreateMosque("Mosque A");
        var mosqueB = CreateMosque("Mosque B");
        var examiner = CreateExamer();

        db.AddRange(
            mosqueA,
            mosqueB,
            examiner,
            CreateBook("Book A", mosqueA.Id),
            CreateBook("Book B", mosqueB.Id));
        await db.SaveChangesAsync();

        var response = await CreateService(db, examiner.Id)
            .GetAllBooksAsync(new GetAllBooksRequest { PageNumber = 1, PageSize = 20 });
        var books = Assert.IsType<List<BookResponseDto>>(response.Data);

        Assert.True(response.Success);
        Assert.Empty(books);
    }

    [Fact]
    public async Task Examer_CannotAddUpdateOrDeleteBooks()
    {
        await using var db = CreateContext();
        var mosque = CreateMosque("Mosque A");
        var examiner = CreateExamer();
        var book = CreateBook("Existing book", mosque.Id);

        db.AddRange(mosque, examiner, book);
        await db.SaveChangesAsync();

        var service = CreateService(db, examiner.Id);
        var add = await service.AddBookAsync(new AddBookRequest
        {
            BookData = new BookDto
            {
                Title = "New book",
                Author = "Examer",
                Description = "Description",
                CoverImageUrl = "https://example.test/book.pdf"
            },
            BookFile = Array.Empty<byte>()
        });
        var update = await service.UpdateBookInfoAsync(new UpdateBookRequest
        {
            Id = book.Id,
            BookData = new BookDto { Title = "Changed", Author = "Examer", Description = "Changed" }
        });
        var delete = await service.DeleteBookAsync(new DeleteBookRequest { BookId = book.Id });

        Assert.Equal(401, add.StatusCode);
        Assert.Equal(401, update.StatusCode);
        Assert.Equal(401, delete.StatusCode);
        Assert.NotNull(await db.PdfFiles.FindAsync(book.Id));
    }

    private static LibraryService CreateService(AppDbContext db, Guid examinerId)
        => new(new UnitOfWork(db), new FakeCurrentUserService(examinerId), new FakeFileService());

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new AppDbContext(options);
    }

    private static Mosque CreateMosque(string name) => new()
    {
        Id = Guid.NewGuid(),
        name = name,
        address = name
    };

    private static User CreateExamer() => new()
    {
        Id = Guid.NewGuid(),
        name = "Examer",
        UserName = $"examer-{Guid.NewGuid():N}@test.local",
        Email = $"examer-{Guid.NewGuid():N}@test.local",
        role = (int)Roles.Examer,
        created_at = DateTime.UtcNow,
        JoinedAt = DateTime.UtcNow
    };

    private static PdfFile CreateBook(string title, Guid mosqueId) => new()
    {
        Id = Guid.NewGuid(),
        MosqueId = mosqueId,
        SaturdayLessonId = Guid.NewGuid(),
        FileUrl = $"uploads/{Guid.NewGuid():N}.pdf",
        title = title,
        description = "Description",
        uploaded_by = "Supervisor",
        created_at = DateTime.UtcNow
    };

    private sealed class FakeCurrentUserService(Guid examinerId) : ICurrentUserService
    {
        public Guid? CurrentUserId => examinerId;
        public string CurrentUserName => "Examer";
        public bool? IsActived => true;
        public bool? IsAdmin => false;
        public bool IsInRole(string roleName)
            => string.Equals(roleName, Roles.Examer.ToString(), StringComparison.OrdinalIgnoreCase);
        public string GetBaseUrl(string relativePath) => relativePath;
    }

    private sealed class FakeFileService : IFileService
    {
        public Task<string> UploadFileAsync(FilePathType fileType, Guid ownerId, string fileName, byte[] fileData)
            => Task.FromResult(fileName);
        public Task<bool> DeleteFileAsync(string filePath) => Task.FromResult(true);
        public Task<string> GetFileUrlAsync(string filePath) => Task.FromResult(filePath);
    }
}
