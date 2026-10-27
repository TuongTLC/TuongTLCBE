using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Tests;

public class ModelValidationTests
{
    [Fact]
    public void Enums_HaveExpectedValues()
    {
        Assert.Equal("banned", Enums.POST_BANNED);
        Assert.Equal("review", Enums.POST_REVIEW);
        Assert.Equal("approved", Enums.POST_APPROVED);
    }

    [Fact]
    public void PaginationResponseModel_Fluent_AllSetters()
    {
        var model = new PaginationResponseModel()
            .PageSize(20)
            .CurPage(3)
            .RecordCount(150)
            .PageCount(8);
        Assert.Equal(20, model.pageSize);
        Assert.Equal(3, model.curPage);
        Assert.Equal(150, model.recordCount);
        Assert.Equal(8, model.pageCount);
    }

    [Fact]
    public void UserModels_Defaults()
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Username = "test",
            PasswordHash = Array.Empty<byte>(),
            PasswordSalt = Array.Empty<byte>(),
            RoleName = "User",
            Fullname = "Test",
            Email = "test@test.com"
        };
        Assert.Equal("test", user.Username);
        Assert.Equal("User", user.RoleName);
    }

    [Fact]
    public void PostModels_Properties()
    {
        var post = new PostRequestModel
        {
            PostName = "Test Post",
            Content = "Content",
            CategoriesIds = new List<Guid>(),
            TagsIds = new List<Guid>()
        };
        Assert.Equal("Test Post", post.PostName);
        Assert.NotNull(post.CategoriesIds);
        Assert.NotNull(post.TagsIds);
    }

    [Fact]
    public void PostCommentModels_Properties()
    {
        var comment = new PostCommentInsertModel
        {
            PostId = Guid.NewGuid(),
            CommentContent = "Hello",
            ParentCommentId = null
        };
        Assert.Equal("Hello", comment.CommentContent);
        Assert.Null(comment.ParentCommentId);
    }

    [Fact]
    public void CategoryTagModels_Properties()
    {
        var cat = new CategoryModel { CategoryName = "Tech", Description = "Tech news" };
        var tag = new TagModel { TagName = "CSharp", Description = "C#" };
        Assert.Equal("Tech", cat.CategoryName);
        Assert.Equal("CSharp", tag.TagName);
    }
}
