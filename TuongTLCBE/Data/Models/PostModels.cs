using System.Text.Json.Serialization;

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

public class ChangePostStatusModel
{
    public Guid PostId { get; set; }
    public string Status { get; set; }
}

public class PostModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("postName")] public string PostName { get; set; } = null!;

    [JsonPropertyName("summary")] public string? Summary { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("createDate")] public DateTime CreateDate { get; set; }

    [JsonPropertyName("author")] public PostAuthor? Author { get; set; }

    [JsonPropertyName("like")] public int? Like { get; set; }

    [JsonPropertyName("dislike")] public int? Dislike { get; set; }

    [JsonPropertyName("thumbnail")] public string? Thumbnail { get; set; }

    [JsonPropertyName("status")] public bool Status { get; set; }

    [JsonPropertyName("adminStatus")] public string AdminStatus { get; set; }
}

public class PostAuthor
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("fullname")] public string Fullname { get; set; }

    [JsonPropertyName("roleName")] public string RoleName { get; set; }
}

public class PostCategoryModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("categoryName")] public string CategoryName { get; set; } = null!;

    [JsonPropertyName("description")] public string? Description { get; set; }
}

public class PostTagModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("tagName")] public string TagName { get; set; } = null!;

    [JsonPropertyName("description")] public string? Description { get; set; }
}

public class PostInfoModel
{
    [JsonPropertyName("postInfo")] public PostModel PostInfo { get; set; }

    [JsonPropertyName("postCategories")] public List<PostCategoryModel> PostCategories { get; set; }

    [JsonPropertyName("postTags")] public List<PostTagModel> PostTags { get; set; }
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