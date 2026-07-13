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

public class TeacherLibraryAccessTests
{
    [Fact]
    public async Task Teacher_SeesOnlyBooksFromAssignedMosque()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var teacher = CreateTeacher(Guid.NewGuid(), mosqueA);
        var ownBook = CreateBook("Own mosque book", mosqueA.Id);
        var otherBook = CreateBook("Other mosque book", mosqueB.Id);

        db.AddRange(mosqueA, mosqueB, teacher, ownBook, otherBook);
        await db.SaveChangesAsync();

        var service = CreateService(db, teacher.Id);
        var response = await service.GetAllBooksAsync(new GetAllBooksRequest { PageNumber = 1, PageSize = 20 });
        var books = Assert.IsType<List<BookResponseDto>>(response.Data);

        Assert.True(response.Success);
        Assert.Single(books);
        Assert.Equal(ownBook.title, books[0].Title);
        Assert.DoesNotContain(books, book => book.Title == otherBook.title);
    }

    [Fact]
    public async Task Teacher_CannotAccessBookFromAnotherMosque()
    {
        await using var db = CreateContext();
        var mosqueA = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var mosqueB = new Mosque { Id = Guid.NewGuid(), name = "Mosque B", address = "B" };
        var teacher = CreateTeacher(Guid.NewGuid(), mosqueA);
        var otherBook = CreateBook("Other mosque book", mosqueB.Id);

        db.AddRange(mosqueA, mosqueB, teacher, otherBook);
        await db.SaveChangesAsync();

        var response = await CreateService(db, teacher.Id)
            .GetBookByIdAsync(new GetBookByIdRequest { BookId = otherBook.Id });

        Assert.False(response.Success);
        Assert.Equal(401, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_CannotAddUpdateOrDeleteBooks()
    {
        await using var db = CreateContext();
        var mosque = new Mosque { Id = Guid.NewGuid(), name = "Mosque A", address = "A" };
        var teacher = CreateTeacher(Guid.NewGuid(), mosque);
        var book = CreateBook("Existing book", mosque.Id);

        db.AddRange(mosque, teacher, book);
        await db.SaveChangesAsync();

        var service = CreateService(db, teacher.Id);
        var add = await service.AddBookAsync(new AddBookRequest
        {
            BookData = new BookDto
            {
                Title = "New book",
                Author = "Supervisor",
                Description = "Description",
                CoverImageUrl = "https://example.test/book.pdf"
            },
            BookFile = Array.Empty<byte>()
        });
        var update = await service.UpdateBookInfoAsync(new UpdateBookRequest
        {
            Id = book.Id,
            BookData = new BookDto { Title = "Changed", Author = "Teacher", Description = "Changed" }
        });
        var delete = await service.DeleteBookAsync(new DeleteBookRequest { BookId = book.Id });

        Assert.Equal(401, add.StatusCode);
        Assert.Equal(401, update.StatusCode);
        Assert.Equal(401, delete.StatusCode);
        Assert.NotNull(await db.PdfFiles.FindAsync(book.Id));
    }

    private static LibraryService CreateService(AppDbContext db, Guid teacherId)
        => new(new UnitOfWork(db), new FakeCurrentUserService(teacherId), new FakeFileService());

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new AppDbContext(options);
    }

    private static Teacher CreateTeacher(Guid id, Mosque mosque) => new()
    {
        Id = id,
        name = "Teacher",
        UserName = $"{id:N}@test.local",
        Email = $"{id:N}@test.local",
        role = 1,
        status = 0,
        MosqueId = mosque.Id,
        Mosque = mosque,
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

    private sealed class FakeCurrentUserService(Guid teacherId) : ICurrentUserService
    {
        public Guid? CurrentUserId => teacherId;
        public string CurrentUserName => "Teacher";
        public bool? IsActived => true;
        public bool? IsAdmin => false;
        public bool IsInRole(string roleName) => string.Equals(roleName, nameof(Roles.Teacher), StringComparison.OrdinalIgnoreCase);
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
