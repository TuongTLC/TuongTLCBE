namespace TuongTLCBE.Data.Models;

public class PostRequestModel
{
    public string PostName { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; }
    public string? Thumbnail { get; set; }
    public List<Guid> CategoriesIds { get; set; }
    public List<Guid> TagsIds { get; set; }
}

public class PostModel
{
    public Guid Id { get; set; }
    public string PostName { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public PostAuthor? Author { get; set; }
    public int? Like { get; set; }
    public int? Dislike { get; set; }
    public string? Thumbnail { get; set; }
    public bool Status { get; set; }
}

public class PostAuthor
{
    public Guid Id { get; set; }
    public string Fullname { get; set; }
    public string RoleName { get; set; }
}

public class PostCategoryModel
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}

public class PostTagModel
{
    public Guid Id { get; set; }
    public string TagName { get; set; } = null!;
    public string? Description { get; set; }
}

public class PostInfoModel
{
    public PostModel PostInfo { get; set; }
    public List<PostCategoryModel> PostCategories { get; set; }
    public List<PostTagModel> PostTags { get; set; }
}

public class PostPagingResponseModel
{
    public object Paging { get; set; }
    public object ListPosts { get; set; }
}

public class PostUpdateModel
{
    public Guid Id { get; set; }
    public string PostName { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; }
    public string? Thumbnail { get; set; }
    public List<Guid> CategoriesIds { get; set; }
    public List<Guid> TagsIds { get; set; }
}