using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Tests;

public class RepositoryInMemoryTests
{
    private TuongTLCDBContext GetContext()
    {
        var options = new DbContextOptionsBuilder<TuongTLCDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TuongTLCDBContext(options);
    }

    [Fact]
    public async Task CategoryRepo_Get_Works()
    {
        using var context = GetContext();
        var id = Guid.NewGuid();
        context.Categories.Add(new Category { Id = id, CategoryName = "Tech", Description = "Tech" });
        await context.SaveChangesAsync();
        var repo = new CategoryRepo(context);
        var result = await repo.Get(id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TagRepo_Get_Works()
    {
        using var context = GetContext();
        var id = Guid.NewGuid();
        context.Tags.Add(new Tag { Id = id, TagName = "C#", Description = "CSharp", CreatedDate = DateTime.Now });
        await context.SaveChangesAsync();
        var repo = new TagRepo(context);
        var result = await repo.Get(id);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task FileRepo_GetFilesByUser_Works()
    {
        using var context = GetContext();
        var userId = Guid.NewGuid();
        context.FileUploads.Add(new FileUpload { Id = Guid.NewGuid(), Path = "test.jpg", UploadedBy = userId, UploadDate = DateTime.Now });
        await context.SaveChangesAsync();
        var repo = new FileRepo(context);
        var result = await repo.GetFilesByUser(userId);
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task OTPCodeRepo_GetOtp_Works()
    {
        using var context = GetContext();
        context.Otpcodes.Add(new Otpcode { Id = Guid.NewGuid(), Code = "123456", Email = "test@test.com" });
        await context.SaveChangesAsync();
        var repo = new OTPCodeRepo(context);
        var result = await repo.GetOtp("123456", "test@test.com");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task PostCommentRepo_UpdateComment_Works()
    {
        using var context = GetContext();
        var commentId = Guid.NewGuid();
        context.PostComments.Add(new PostComment { Id = commentId, Content = "Old", Status = true, CommentDate = DateTime.Now });
        await context.SaveChangesAsync();
        var repo = new PostCommentRepo(context, new UserRepo(context), new UserInteractCommentRepo(context));
        var result = await repo.UpdateComment(new TuongTLCBE.Data.Models.PostCommentUpdateModel { CommentId = commentId, Content = "New" });
        Assert.True(result);
    }
}
