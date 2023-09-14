namespace TuongTLCBE.Data.Models;

public class CategoryModel
{
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}
public class CategoryInsertModel
{
    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }
}
public class CategoryUpdateModel
{
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }
}