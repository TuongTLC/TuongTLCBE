using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Tests;

public class DataEntitiesTests
{
    [Fact] public void Category() { var e = new Category { Id = Guid.NewGuid(), CategoryName = "c" }; Assert.NotNull(e); }
    [Fact] public void FileUpload() { var e = new FileUpload { Id = Guid.NewGuid(), Path = "p" }; Assert.NotNull(e); }
    [Fact] public void Otpcode() { var e = new Otpcode { Id = Guid.NewGuid(), Code = "123456", Email = "e" }; Assert.NotNull(e); }
    [Fact] public void Post() { var e = new Post { Id = Guid.NewGuid(), PostName = "p", Content = "c" }; Assert.NotNull(e); }
    [Fact] public void PostCategory() { var e = new PostCategory { Id = Guid.NewGuid() }; Assert.NotNull(e); }
    [Fact] public void PostComment() { var e = new PostComment { Id = Guid.NewGuid(), Content = "c" }; Assert.NotNull(e); }
    [Fact] public void PostTag() { var e = new PostTag { Id = Guid.NewGuid() }; Assert.NotNull(e); }
    [Fact] public void Tag() { var e = new Tag { Id = Guid.NewGuid(), TagName = "t" }; Assert.NotNull(e); }
    [Fact] public void User() { var e = new User { Id = Guid.NewGuid(), Username = "u" }; Assert.NotNull(e); }
    [Fact] public void UserInteractComment() { var e = new UserInteractComment { Id = Guid.NewGuid() }; Assert.NotNull(e); }
    [Fact] public void UserInteractPost() { var e = new UserInteractPost { Id = Guid.NewGuid() }; Assert.NotNull(e); }
    [Fact] public void UserRole() { var e = new UserRole { Id = Guid.NewGuid(), RoleName = "r" }; Assert.NotNull(e); }
}
