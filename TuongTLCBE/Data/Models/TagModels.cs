namespace TuongTLCBE.Data.Models;

public class TagModel
{
    public Guid Id { get; set; }

    public string TagName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? Status { get; set; }
}
public class TagInsertModel
{
    public string TagName { get; set; } = null!;

    public string? Description { get; set; }
}
public class TagUpdateModel
{
    public Guid Id { get; set; }

    public string TagName { get; set; } = null!;

    public string? Description { get; set; }
}

public class PostTagRequestModel
{
    public Guid PostId { get; set; }
    public List<Guid> TagsIds { get; set; }
}