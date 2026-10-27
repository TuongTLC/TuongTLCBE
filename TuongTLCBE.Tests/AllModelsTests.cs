using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Tests;

public class AllModelsTests
{
    [Fact] public void CategoryModel() { var m = new CategoryModel { Id = Guid.NewGuid(), CategoryName = "c", Description = "d", CreatedBy = Guid.NewGuid(), CreatedDate = DateTime.Now, Status = true }; Assert.NotNull(m); }
    [Fact] public void CategoryInsertModel() { var m = new CategoryInsertModel { CategoryName = "c", Description = "d" }; Assert.Equal("c", m.CategoryName); }
    [Fact] public void CategoryUpdateModel() { var m = new CategoryUpdateModel { Id = Guid.NewGuid(), CategoryName = "c", Description = "d" }; Assert.NotNull(m); }
    [Fact] public void TagModel() { var m = new TagModel { Id = Guid.NewGuid(), TagName = "t", Description = "d", CreatedBy = Guid.NewGuid(), CreatedDate = DateTime.Now, Status = true }; Assert.NotNull(m); }
    [Fact] public void TagInsertModel() { var m = new TagInsertModel { TagName = "t", Description = "d" }; Assert.Equal("t", m.TagName); }
    [Fact] public void TagUpdateModel() { var m = new TagUpdateModel { Id = Guid.NewGuid(), TagName = "t", Description = "d" }; Assert.NotNull(m); }
    [Fact] public void PostCategoryRequestModel() { var m = new PostCategoryRequestModel { PostId = Guid.NewGuid(), CategoriesIds = new List<Guid>() }; Assert.NotNull(m); }
    [Fact] public void PostTagRequestModel() { var m = new PostTagRequestModel { PostId = Guid.NewGuid(), TagsIds = new List<Guid>() }; Assert.NotNull(m); }
    [Fact] public void EmailSecretModel() { var m = new EmailSecretModel { Email = "e", Password = "p" }; Assert.Equal("e", m.Email); }
    [Fact] public void UserLoginResponseModel() { var m = new UserLoginResponseModel { Token = "t", UserInfo = new UserInfoModel() }; Assert.Equal("t", m.Token); }
    [Fact] public void BanUserModel() { var m = new BanUserModel { UserId = Guid.NewGuid(), Status = true }; Assert.True(m.Status); }
    [Fact] public void UserChangePasswordRequestModel() { var m = new UserChangePasswordRequestModel { Username = "u", OldPassword = "o", NewPassword = "n" }; Assert.Equal("u", m.Username); }
    [Fact] public void EmailVerifyModel() { var m = new EmailVerifyModel { Code = "c", Username = "u" }; Assert.Equal("c", m.Code); }
    [Fact] public void PostPagingResponseModel() { var m = new PostPagingResponseModel { Paging = new object(), ListPosts = new object() }; Assert.NotNull(m); }
    [Fact] public void PostUpdateModel() { var m = new PostUpdateModel { Id = Guid.NewGuid(), PostName = "p", Content = "c", CategoriesIds = new List<Guid>(), TagsIds = new List<Guid>() }; Assert.NotNull(m); }
    [Fact] public void ChangePostStatusModel() { var m = new ChangePostStatusModel { PostId = Guid.NewGuid(), Status = "active" }; Assert.Equal("active", m.Status); }
    [Fact] public void PostAuthor() { var m = new PostAuthor { Id = Guid.NewGuid(), Fullname = "f", RoleName = "r" }; Assert.Equal("f", m.Fullname); }
    [Fact] public void PostModel() { var m = new PostModel { Id = Guid.NewGuid(), PostName = "p", Content = "c", CreateDate = DateTime.Now, Status = true, AdminStatus = "approved" }; Assert.Equal("p", m.PostName); }
    [Fact] public void PostCategoryModel() { var m = new PostCategoryModel { Id = Guid.NewGuid(), CategoryName = "c" }; Assert.Equal("c", m.CategoryName); }
    [Fact] public void PostTagModel() { var m = new PostTagModel { Id = Guid.NewGuid(), TagName = "t" }; Assert.Equal("t", m.TagName); }
    [Fact] public void PostInfoModel() { var m = new PostInfoModel { PostInfo = new PostModel(), PostCategories = new List<PostCategoryModel>(), PostTags = new List<PostTagModel>() }; Assert.NotNull(m); }
    [Fact] public void PostCommentModel() { var m = new PostCommentModel { Id = Guid.NewGuid(), Commenter = new PostCommenter(), PostId = Guid.NewGuid(), Content = "c", Replies = new List<PostCommentModel>() }; Assert.Equal("c", m.Content); }
    [Fact] public void PostCommenter() { var m = new PostCommenter { Id = Guid.NewGuid(), CommenterName = "n", Username = "u" }; Assert.Equal("n", m.CommenterName); }
    [Fact] public void PostCommentUpdateModel() { var m = new PostCommentUpdateModel { CommentId = Guid.NewGuid(), Content = "c", Status = true }; Assert.True(m.Status); }
}
